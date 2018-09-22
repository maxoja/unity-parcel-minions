using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoardData))]
public class BotRouter : MonoBehaviour
{
    public static BotRouter instance;

    BoardData _boardData;
    BoardData boardData { get { if (_boardData == null) _boardData = GetComponent<BoardData>(); return _boardData; } }
    Queue<Vector2Int> traverseQueue = new Queue<Vector2Int>();
    int[,] traverseMap = new int[BoardData.height, BoardData.width];

    private Vector2Int[] directions = new Vector2Int[4] { 
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1) 
    };

    public Vector2Int RequestNextStep(ParcelBot bot)
    {
        Vector2Int target = bot.GetTarget();
        Vector2Int current = bot.GetCurrent();

        Vector2Int nextStep = CalculateNextStep(bot, current, target);

        return nextStep;
        
    }

    private Vector2Int CalculateNextStep(ParcelBot bot, Vector2Int current, Vector2Int target)
    {
        FetchTraverseMap(bot);
        if (traverseMap[target.y, target.x] == -2)
            return current;
        traverseMap[current.y, current.x] = 0;
        traverseQueue.Enqueue(current);

        while(traverseQueue.Count > 0)
        {
            Vector2Int now = traverseQueue.Dequeue();
            foreach (Vector2Int next in GetNextPossibleDestinations(now))
                traverseQueue.Enqueue(next);
            traverseMap[now.y, now.x] = GetBestValueFromAround(now)+1;
        }

        //DebugTraverseMap();

        if (traverseMap[target.y, target.x] < 0)
            return current;
        
        Vector2Int tracker = target;
        Vector2Int prev = tracker;
        while(tracker!=current)
        {
            prev = tracker;
            tracker = GetNextBackStep(tracker);
        }

        return prev;
    }

    private void FetchTraverseMap(ParcelBot bot)
    {
        for (int i = 0; i < BoardData.height;i++)
        {
            for (int j = 0; j < BoardData.width;j++)
            {
                BoardData.CellType staticCell = BoardData.instance.GetCellType(i, j);
                traverseMap[i, j] = staticCell == BoardData.CellType.ParcelHole ? -2 : -1;
            }
        }

        foreach(ParcelBot b in BoardData.instance.GetBots())
        {
            if (b == bot)
                continue;

            Vector2Int bCurrent = b.GetCurrent();
            Vector2Int bNextStep = b.GetNextStep();
            traverseMap[bCurrent.y, bCurrent.x] = -2;
            traverseMap[bNextStep.y, bNextStep.x] = -2;
        }
    }

    private void DebugTraverseMap()
    {
        //debug
        string r = "";
        for (int i = 0; i < BoardData.height;i++)
        {
            for (int j = 0; j < BoardData.width; j++)
                r += traverseMap[i, j] + ", ";
            r += "\n";
        }
        Debug.Log(r);   
    }

    private List<Vector2Int> GetNextPossibleDestinations(Vector2Int current)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int i = 0; i < directions.Length;i++)
        {
            Vector2Int newDes = current + directions[i];
            if (newDes.x < 0 || newDes.y<0)
                continue;
            if (newDes.x >= BoardData.width || newDes.y >= BoardData.height)
                continue;
            if (traverseMap[newDes.y, newDes.x] != -1)
                continue;
            result.Add(newDes);
        }

        return result;
    }

    private int GetBestValueFromAround(Vector2Int current)
    {
        int min = 999999;

        if (traverseMap[current.y, current.x] == 0)
            return 0;
        
        foreach(Vector2Int dir in directions)
        {
            Vector2Int cell = current + dir;

            if (cell.x < 0 || cell.y < 0)
                continue;
            if (cell.x >= BoardData.width || cell.y >= BoardData.height)
                continue;

            int mapVal = traverseMap[cell.y, cell.x];
            if (mapVal < 0 || mapVal >= min)
                continue;

            min = mapVal;
        }

        return min;
    }

    private Vector2Int GetNextBackStep(Vector2Int current)
    {
        int min = 999999;
        Vector2Int bestStep = Vector2Int.zero;

        foreach (Vector2Int dir in directions)
        {
            Vector2Int cell = current + dir;

            if (cell.x < 0 || cell.y < 0)
                continue;
            if (cell.x >= BoardData.width || cell.y >= BoardData.height)
                continue;

            int mapVal = traverseMap[cell.y, cell.x];
            if (mapVal < 0 || mapVal >= min)
                continue;

            min = mapVal;
            bestStep = cell;
        }

        return bestStep;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(ParcelBot b in BoardData.instance.GetBots())
        {
            b.AssignNextParcelPoint(new Vector2Int(13,13));
        }
    }
}

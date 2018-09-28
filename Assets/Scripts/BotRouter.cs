using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(BoardData))]
public class BotRouter : MonoBehaviour
{
    public static BotRouter instance;

    BoardData _boardData;
    BoardData boardData { get { if (_boardData == null) _boardData = GetComponent<BoardData>(); return _boardData; } }

    private Vector2Int[] directions = new Vector2Int[4] { 
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1) 
    };

    private void Awake()
    {
        instance = this; // make this class accessible from anywhere
    }

    public IEnumerator RequestNextStep(ParcelBot bot)
    {
        Vector2Int target = bot.GetTarget();
        Vector2Int current = bot.GetCurrent();

        //start the algorithm
        yield return StartCoroutine(CalculateNextStep(bot, current, target));
        
    }

    private IEnumerator CalculateNextStep(ParcelBot bot, Vector2Int current, Vector2Int target)
    {
        //create temp map arrays and traverse queue for calculation
        int[,] traverseMap = new int[BoardData.height, BoardData.width];
        Queue<Vector2Int> traverseQueue = new Queue<Vector2Int>();

        //fill values into map arrays
        FetchTraverseMap(bot,ref traverseMap);

        //if the target cell is not available (not empty)
        if (traverseMap[target.y, target.x] == -2)
        {
            //stop bot movement
            bot.SetNextStep(current);
            yield break;
        }

        //initialize first value in the queue
        traverseMap[current.y, current.x] = 0;
        traverseQueue.Clear();
        traverseQueue.Enqueue(current);

        //while the queue is not empty
        while(traverseQueue.Count > 0)
        {
            Vector2Int now = traverseQueue.Dequeue();

            //add nearby cell's position that bot can walk to to the queue for next round of calculation
            AddPossibleNextStepToQueue(now, target, ref traverseMap, ref traverseQueue);
            //get the shortest cost to travel from 'current' to 'now'
            traverseMap[now.y, now.x] = GetBestValueFromAround(now, ref traverseMap)+1;
            //stop loop when reached target cell
            if (traverseMap[target.y, target.x] != -1)
                break;
        }

        //DebugTraverseMap();

        //if the target is not reached
        if (traverseMap[target.y, target.x] < 0)
        {
            //stop bot movement
            bot.SetNextStep(current);
            yield break;
        }

        //initialize values to finc next step from calculated map
        Vector2Int tracker = target;    //stat from target cell
        Vector2Int prev = tracker;      //this var is to temporarily keep tracking position

        //do the block until tracker reached start cell
        while(tracker!=current)
        {
            prev = tracker;
            //move tracker one step closer to start point
            tracker = GetNextBackStep(tracker, ref traverseMap);
        }

        //tell bot to move to the cell before the tracker reached bot
        bot.SetNextStep(prev);
    }

    private void FetchTraverseMap(ParcelBot bot, ref int[,] traverseMap)
    {
        //loop over every cell
        for (int i = 0; i < BoardData.height;i++)
        {
            for (int j = 0; j < BoardData.width;j++)
            {
                BoardData.CellType staticCell = BoardData.instance.GetCellType(i, j);

                //if the cell is parcel hole, the value will be -2 else -1
                //-2 means blocked
                //-1 means walkable
                traverseMap[i, j] = staticCell == BoardData.CellType.ParcelHole ? -2 : -1;
            }
        }

        //loop over everybot except current bot that we are going to assign direction
        foreach(ParcelBot b in BoardData.instance.GetBots())
        {
            if (b == bot)
                continue;

            //set value -2 to the cell that each bot is standing or stepping to
            Vector2Int bCurrent = b.GetCurrent();
            Vector2Int bNextStep = b.GetNextStep();
            traverseMap[bCurrent.y, bCurrent.x] = -2;
            traverseMap[bNextStep.y, bNextStep.x] = -2;
        }
    }

    //print the map for debugging
    private void DebugTraverseMap(ref int[,] traverseMap)
    {
        string r = "";
        for (int i = 0; i < BoardData.height;i++)
        {
            for (int j = 0; j < BoardData.width; j++)
                r += traverseMap[i, j] + ", ";
            r += "\n";
        }
        Debug.Log(r);   
    }

    private void AddPossibleNextStepToQueue(Vector2Int current, Vector2Int target,ref int[,] traverseMap, ref Queue<Vector2Int> traverseQueue)
    {
        Vector2Int diff = target - current;
        int dist = Mathf.Abs(current.x - target.x) + Mathf.Abs(current.y - target.y);

        //loop over every direction possible
        //in this case, left right up down
        for (int i = 0; i < directions.Length;i++)
        {
            Vector2Int newDes = current + directions[i];

            //if the next position is out of board
            //or blocked by a parcel hole or a bot
            //or has been taken to the calculation
            //then skip the position
            if (newDes.x < 0 || newDes.y<0)
                continue;
            if (newDes.x >= BoardData.width || newDes.y >= BoardData.height)
                continue;
            if (traverseMap[newDes.y, newDes.x] != -1)
                continue;
            if (traverseQueue.Contains(newDes))
                continue;
            
            //other wise, this position should be calculated
            //then add this position to the traverseQueue
            traverseQueue.Enqueue(newDes);
        }
    }

    //return the value of shortest distance to the current position in the map
    private int GetBestValueFromAround(Vector2Int current, ref int[,] traverseMap)
    {
        int min = 999999; //initialize min value

        if (traverseMap[current.y, current.x] == 0)
            return 0;

        //loop ever every direction (left, right, up, down)
        foreach(Vector2Int dir in directions)
        {
            //if the value in the cell next to the current cell has smaller value
            //then set it to min

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

    //get the next position that tracker should walk to
    private Vector2Int GetNextBackStep(Vector2Int current, ref int[,] traverseMap)
    {
        int min = 999999;
        Vector2Int bestStep = Vector2Int.zero;

        //loop around current position
        foreach (Vector2Int dir in directions)
        {
            //find the smallest neary by cell and save value, position into min and bestStep
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

        //return the nearest position with minimum value in the cell
        return bestStep;
    }
}

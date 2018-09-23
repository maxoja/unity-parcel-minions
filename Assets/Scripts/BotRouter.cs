﻿using UnityEngine;
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

    public IEnumerator RequestNextStep(ParcelBot bot)
    {
        Vector2Int target = bot.GetTarget();
        Vector2Int current = bot.GetCurrent();

        yield return StartCoroutine(CalculateNextStep(bot, current, target));
        
    }

    private IEnumerator CalculateNextStep(ParcelBot bot, Vector2Int current, Vector2Int target)
    {
        int[,] traverseMap = new int[BoardData.height, BoardData.width];
        Queue<Vector2Int> traverseQueue = new Queue<Vector2Int>();

        FetchTraverseMap(bot,ref traverseMap);
        //yield return null;
        if (traverseMap[target.y, target.x] == -2)
        {
            bot.SetNextStep(current);
            yield break;
        }

        traverseMap[current.y, current.x] = 0;
        traverseQueue.Clear();
        traverseQueue.Enqueue(current);

        int i = 1;
        while(traverseQueue.Count > 0)
        {
            Vector2Int now = traverseQueue.Dequeue();
            AddPossibleNextStepToQueue(now, target, ref traverseMap, ref traverseQueue);
            traverseMap[now.y, now.x] = GetBestValueFromAround(now, ref traverseMap)+1;
            if (traverseMap[target.y, target.x] != -1)
                break;
            i += 1;
            //if (i % 50 == 0)
                //yield return null;
        }
        //print(i);
        //DebugTraverseMap();

        if (traverseMap[target.y, target.x] < 0)
        {
            bot.SetNextStep(current);
            yield break;
        }
        
        Vector2Int tracker = target;
        Vector2Int prev = tracker;
        while(tracker!=current)
        {
            prev = tracker;
            tracker = GetNextBackStep(tracker, ref traverseMap);
        }

        bot.SetNextStep(prev);
    }

    private void FetchTraverseMap(ParcelBot bot, ref int[,] traverseMap)
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

    private void DebugTraverseMap(ref int[,] traverseMap)
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

    private void AddPossibleNextStepToQueue(Vector2Int current, Vector2Int target,ref int[,] traverseMap, ref Queue<Vector2Int> traverseQueue)
    {
        Vector2Int diff = target - current;
        int dist = Mathf.Abs(current.x - target.x) + Mathf.Abs(current.y - target.y);

        for (int i = 0; i < directions.Length;i++)
        {
            if (dist >= 5)
            {
                if (diff.y > 0 && directions[i] == Vector2Int.down)
                    continue;
                else if (diff.y < 0 && directions[i] == Vector2Int.up)
                    continue;
            }
            Vector2Int newDes = current + directions[i];
            if (newDes.x < 0 || newDes.y<0)
                continue;
            if (newDes.x >= BoardData.width || newDes.y >= BoardData.height)
                continue;
            if (traverseMap[newDes.y, newDes.x] != -1)
                continue;
            if (traverseQueue.Contains(newDes))
                continue;
            traverseQueue.Enqueue(newDes);
        }
    }

    private int GetBestValueFromAround(Vector2Int current, ref int[,] traverseMap)
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

    private Vector2Int GetNextBackStep(Vector2Int current, ref int[,] traverseMap)
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
        //foreach(ParcelBot b in BoardData.instance.GetBots())
        //{
        //    b.AssignNextParcelPoint(new Vector2Int(10,10));
        //}
    }
}

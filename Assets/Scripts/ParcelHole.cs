using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelHole : MonoBehaviour {
    private int x, y;
    private List<Vector2Int> nearbyBlocks = new List<Vector2Int>();

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public void SetPosition(Vector2Int pos)
    {
        x = pos.x;
        y = pos.y;
        nearbyBlocks.Clear();
        nearbyBlocks.Add(pos + Vector2Int.left);
        nearbyBlocks.Add(pos + Vector2Int.right);
        nearbyBlocks.Add(pos + Vector2Int.up);
        nearbyBlocks.Add(pos + Vector2Int.down);
    }

    private void Update()
    {
        transform.localScale = Vector3.one * BoardData.gridScale;
        transform.position = new Vector2(x, y) * BoardData.gridScale;
    }

    private void OnMouseDown()
    {
        ParcelBot responsibleBot = null;
        List<ParcelBot> allBots = BoardData.instance.GetBots();
        List<Vector2Int> availableBlocks = new List<Vector2Int>(nearbyBlocks);

        int minLoad = 999999;
        ParcelBot minBot = null;
        foreach(ParcelBot bot in allBots)
        {
            if (bot.GetLoadCount() < minLoad)
            {
                minLoad = bot.GetLoadCount();
                minBot = bot;
            }

            if (responsibleBot == null && bot.IsFree())
                responsibleBot = bot;

            if (availableBlocks.Contains(bot.GetTarget()))
                availableBlocks.Remove(bot.GetTarget());
        }

        if (responsibleBot == null)
            responsibleBot = minBot;

        if (availableBlocks.Count == 0)
            availableBlocks.Add(nearbyBlocks[Random.Range(0, 3)]);

        print("assign " + availableBlocks[0].ToString());
        responsibleBot.AssignNextParcelPoint(availableBlocks[0]);
    }
}

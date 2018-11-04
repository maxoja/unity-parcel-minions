using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParcelHole : MonoBehaviour {
    private int x, y, id;
    private List<Vector2Int> nearbyBlocks = new List<Vector2Int>();
    private Text textComp;

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

    //this will be called after a hole is created
    public void Initialize(Vector2Int pos, int id)
    {
        //set id, position, text
        this.id = id;
        textComp.text = id.ToString();
        x = pos.x;
        y = pos.y;

        //generate nearby (left right up down) block position
        nearbyBlocks.Clear();
        nearbyBlocks.Add(pos + Vector2Int.left);
        nearbyBlocks.Add(pos + Vector2Int.right);
        nearbyBlocks.Add(pos + Vector2Int.up);
        nearbyBlocks.Add(pos + Vector2Int.down);
    }

    private void Awake()
    {
        textComp = GetComponentInChildren<Text>();
    }
    private void Update()
    {
        transform.localScale = Vector3.one * BoardData.gridScale;
        transform.position = new Vector2(x, y) * BoardData.gridScale;
    }

    //this method will be called when clicked
    private void OnMouseDown()
    {
        //find the bot with the minimum load
        //find the block with minimum load

        ParcelBot responsibleBot = null;
        List<ParcelBot> allBots = BoardData.instance.GetBots();
        List<Vector2Int> availableBlocks = new List<Vector2Int>(nearbyBlocks);

        int minLoad = 999999;
        ParcelBot minBot = null;
        foreach(ParcelBot bot in allBots)
        {
            //int load = bot.GetLoadCount();
            int load = bot.GetLoadCount() + Mathf.Abs(bot.GetId() - x) + Mathf.Abs(y);
            if (bot.IsFree() == false) load += 100;
            //float load = (float)(bot.GetLoadCount() + Mathf.Abs(bot.GetCurrent().x - x) + Mathf.Abs(bot.GetCurrent().y - y));
            //print(bot.GetCurrent());
            //print(x +" " + y);
            //print(load);
            //print("");

            if (load < minLoad)
            {
                responsibleBot = bot;
                minLoad = load;
                minBot = bot;
            }

            if (responsibleBot == null && bot.IsFree())
                responsibleBot = bot;

            if (availableBlocks.Contains(bot.GetTarget()))
                availableBlocks.Remove(bot.GetTarget());
        }

        //forget what this condition does but don't dare to remove it ;-)
        if (responsibleBot == null)
            responsibleBot = minBot;

        //no bot with enough battery available
        if (responsibleBot == null)
            return;

        //if there is no available nearby block that is currently free
        //just pick one randomly
        if (availableBlocks.Count == 0)
            availableBlocks.Add(nearbyBlocks[Random.Range(0, 3)]);

        //assign the bot to move to the nearby block with minimum load
        responsibleBot.AssignNextParcelPoint(availableBlocks[0],id);
    }
}

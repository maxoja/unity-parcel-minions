using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardData))]
public class ParcelBotGenerator : MonoBehaviour {
    public ParcelBot prefab;

    public void Start()
    {
        BoardData.instance.ClearAllBots();
        for (int i = 0; i < BoardData.botNum;i++)
        {
            ParcelBot bot = Instantiate(prefab) as ParcelBot;
            bot.transform.parent = transform;
            bot.Initialize(i);
            BoardData.instance.AddBot(bot);
        }
    }
}

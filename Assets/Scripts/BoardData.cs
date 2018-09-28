using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BoardData : MonoBehaviour 
{
    static public BoardData instance = null;

    public enum CellType : int
    {
        ParcelHole,
        ChargingArea,
        WalkingArea
    }

    public const float gridScale = 1.2f;

    //public const int botNum = 11;
    //public const int width = 11;
    //public const int height = 11;
    //private const string boardObjectStringMap =
        //"..........." +
        //"..........." +
        //"..x..x..x.." +
        //"..........." +
        //"..........." +
        //"..x..x..x.." +
        //"..........." +
        //"..........." +
        //"..x..x..x.." +
        //"..........." +
        //"___________";

    //public const int botNum = 14;
    //public const int width = 14;
    //public const int height = 14;
    //private const string boardObjectStringMap =
        //".............." +
        //".............." +
        //"..x..x..x..x.." +
        //".............." +
        //".............." +
        //"..x..x..x..x.." +
        //".............." +
        //".............." +
        //"..x..x..x..x.." +
        //".............." +
        //".............." +
        //"..x..x..x..x.." +
        //".............." +
        //"______________";

    public const int botNum = 17;
    public const int width = 17;
    public const int height = 17;
    private const string boardObjectStringMap =
        "................." +
        "................." +
        "..x..x..x..x..x.." +
        "................." +
        "................." +
        "..x..x..x..x..x.." +
        "................." +
        "................." +
        "..x..x..x..x..x.." +
        "................." +
        "................." +
        "..x..x..x..x..x.." +
        "................." +
        "................." +
        "..x..x..x..x..x.." +
        "................." +
        "_________________";
    
    private CellType[,] cellMap = new CellType[height,width];
    private int parcelHoleCount = 0;
    private List<Vector2Int> parcelHolePositions;
    private List<ParcelBot> bots;

	void Start () 
    {
        instance = this; // make this component accessible from anywhere

        //initialize values
        parcelHoleCount = 0;
        parcelHolePositions = new List<Vector2Int>();
        bots = new List<ParcelBot>();

        //transform map string to map of enums
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CellType cell = CellType.WalkingArea;
                switch(boardObjectStringMap[i*width+j])
                {
                    case '.': 
                        cell = CellType.WalkingArea;
                        break;
                    case 'x':
                        cell = CellType.ParcelHole;
                        parcelHolePositions.Add(new Vector2Int(j,i));
                        parcelHoleCount++;
                        break;
                    case '_': 
                        cell = CellType.ChargingArea;
                        break;
                }

                cellMap[i, j] = cell;
            }
        }
	}
	
    public int GetParcelHoleCount()
    {
        //return how many parcel holes we have
        return parcelHoleCount;
    }

    public List<Vector2Int> GetParcelHolePositions()
    {
        //return a duplicated list of parcel hole positions
        return new List<Vector2Int>(parcelHolePositions);
    }

    public CellType GetCellType(int i, int j)
    {
        //get the value of map cell at y=i x=j 
        return cellMap[i,j];
    }

    public void ClearAllBots()
    {
        foreach(ParcelBot b in bots)
        {
            if (b != null)
                Destroy(b.gameObject);
        }
        bots.Clear();
    }

    public void AddBot(ParcelBot b)
    {
        //add new bot b to bot list
        bots.Add(b);
    }

    public List<ParcelBot> GetBots()
    {
        //return a duplicated list of existing bots and return
        return new List<ParcelBot>(bots);
    }
}

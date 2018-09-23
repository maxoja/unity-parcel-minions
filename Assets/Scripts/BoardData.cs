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

    //[SerializeField]
    private CellType[,] cellMap = new CellType[height,width];
    //[SerializeField]
    private int parcelHoleCount = 0;
    //[SerializeField]
    private List<Vector2Int> parcelHolePositions;
    //[SerializeField]
    private List<ParcelBot> bots;

	void Start () 
    {
        instance = this;
        parcelHoleCount = 0;
        parcelHolePositions = new List<Vector2Int>();
        bots = new List<ParcelBot>();

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
        return parcelHoleCount;
    }

    public List<Vector2Int> GetParcelHolePositions()
    {
        return new List<Vector2Int>(parcelHolePositions);
    }

    public CellType GetCellType(int i, int j)
    {
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
        bots.Add(b);
    }

    public List<ParcelBot> GetBots()
    {
        return new List<ParcelBot>(bots);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoardData : MonoBehaviour 
{
    enum CellType : int
    {
        ParcelHole,
        ChargingArea,
        WalkingArea
    }

    public const int botNum = 10;
    public const int width = 14;
    public const int height = 14;
    public const float gridScale = 1;

    private const string boardObjectStringMap =
        ".............." +
        ".............." +
        "..x..x..x..x.." +
        ".............." +
        ".............." +
        "..x..x..x..x.." +
        ".............." +
        ".............." +
        "..x..x..x..x.." +
        ".............." +
        ".............." +
        "..x..x..x..x.." +
        ".............." +
        "______________";

    [SerializeField]
    private CellType[,] cellMap = new CellType[height,width];
    [SerializeField]
    private int parcelHoleCount = 0;
    [SerializeField]
    private List<Vector2Int> parcelHolePositions;

	void Start () 
    {
        parcelHoleCount = 0;
        parcelHolePositions = new List<Vector2Int>();

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
}

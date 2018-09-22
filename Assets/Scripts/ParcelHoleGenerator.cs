using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardData))]
//[ExecuteInEditMode]
public class ParcelHoleGenerator : MonoBehaviour 
{
    public ParcelHole prefab;
    private List<ParcelHole> holes;
    private BoardData boardData = null;

    private void Start()
    {
        if (boardData == null)
            boardData = GetComponent<BoardData>();

        foreach (ParcelHole hole in GetComponentsInChildren<ParcelHole>())
            DestroyImmediate(hole.gameObject);
        
        holes = new List<ParcelHole>();

        foreach(Vector2Int p in boardData.GetParcelHolePositions())
        {
            holes.Add(CreateParcelHole(p));   
        }
    }

    private ParcelHole CreateParcelHole(Vector2Int pos)
    {
        ParcelHole hole = Instantiate(prefab) as ParcelHole;
        hole.transform.parent = transform;
        hole.SetPosition(pos);
        return hole;
    }
}

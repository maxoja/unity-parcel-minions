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

        //clear previous holes by destroying it
        foreach (ParcelHole hole in GetComponentsInChildren<ParcelHole>())
            DestroyImmediate(hole.gameObject);
        
        holes = new List<ParcelHole>();

        //create new holes based on staticMap configuration in BoardData
        foreach(Vector2Int p in boardData.GetParcelHolePositions())
        {
            holes.Add(CreateParcelHole(p));
        }
    }

    private ParcelHole CreateParcelHole(Vector2Int pos)
    {
        //create new hole object and set position and id
        ParcelHole hole = Instantiate(prefab) as ParcelHole;
        hole.transform.parent = transform;
        hole.Initialize(pos,holes.Count+1);
        return hole;
    }
}

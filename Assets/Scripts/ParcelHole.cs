using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelHole : MonoBehaviour {
    private int x, y;

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
    }

    private void Update()
    {
        transform.localScale = Vector3.one * BoardData.gridScale;
        transform.position = new Vector2(x, y) * BoardData.gridScale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Board : MonoBehaviour {
    void Update()
    {
        transform.localScale = new Vector2((BoardData.width)*BoardData.gridScale, (BoardData.height)*BoardData.gridScale);
        transform.position = new Vector2((BoardData.width-1) * BoardData.gridScale, (BoardData.height-1) * BoardData.gridScale) / 2;
    }
}

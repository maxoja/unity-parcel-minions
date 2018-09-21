using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChargingArea : MonoBehaviour {
    void Update()
    {
        transform.localScale = new Vector2((BoardData.width) * BoardData.gridScale, BoardData.gridScale);
        transform.position = new Vector2((BoardData.width - 1) * BoardData.gridScale, 0) / 2;
    }
}

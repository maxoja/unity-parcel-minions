using UnityEngine;

//[ExecuteInEditMode]
public class Board : MonoBehaviour {
    void Awake()
    {
        //automatically adjust its size and position according to configurations in BoardData
        transform.localScale = new Vector2((BoardData.width)*BoardData.gridScale, (BoardData.height)*BoardData.gridScale);
        transform.position = new Vector2((BoardData.width-1) * BoardData.gridScale, (BoardData.height-1) * BoardData.gridScale) / 2;
    }
}

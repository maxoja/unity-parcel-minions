using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class ChargingArea : MonoBehaviour {
    //private bool[] slots = new bool[BoardData.botNum];

    //private void Awake()y
    //{
        //for (int i = 0; i < slots.Length; i++)
            //slots[i] = false;
    //}

    void Awake()
    {
        transform.localScale = new Vector2((BoardData.width) * BoardData.gridScale, BoardData.gridScale);
        transform.position = new Vector2((BoardData.width - 1) * BoardData.gridScale, 0) / 2;
    }

    //private int GetAvailableSlot()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //        if (slots[i])
    //            return i;
    //    return -1;
    //}

    //public int TakeAnAvailableSlot()
    //{
    //    int slotId = GetAvailableSlot();

    //    if (slotId != -1)
    //        slots[slotId] = false;
    //    else
    //        Debug.LogError("this can produce a bug, please fix");
        
    //    return slotId;
    //}

    //public void ReturnSlot(int i)
    //{
    //    slots[i] = true;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCamera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        transform.position = new Vector3((BoardData.width-1) * BoardData.gridScale, (BoardData.height-1) * BoardData.gridScale,-20) / 2;
   	}
}

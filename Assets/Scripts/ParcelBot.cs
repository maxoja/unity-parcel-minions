using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelBot : MonoBehaviour 
{
    private int id;
    private Vector2Int target;
    private Vector2Int current;
    private Vector2Int nextStep;

    private Queue<Vector2Int> nextTarget;

    const float speed = 1f;

    private float r = 0;
	
	void Update () 
    {
        if (current == nextStep)
        {
            if (current == target)
            {
                if (nextTarget == null || nextTarget.Count == 0)
                {
                    return;
                }
                else
                {
                    target = nextTarget.Dequeue();
                }
            }
            else
            {
                //request next step
            }
        }

        r += Time.deltaTime;

        Vector2 updatedPosition;
        if(r>=1)
        {
            r -= 1;
            updatedPosition = Vector2.LerpUnclamped(current, nextStep, r+1)*BoardData.gridScale;
            current = target;
        }
        else
        {
            updatedPosition = Vector2.Lerp(current, nextStep, r)*BoardData.gridScale;
        }

        transform.position = updatedPosition;
	}

    public void SetId(int id)
    {
        this.id = id;
    }

    public void AssignNextParcelPoint(Vector2Int assignment)
    {
        nextTarget.Enqueue(assignment);
        nextTarget.Enqueue(new Vector2Int(this.id,0));
    }

    public bool IsFree()
    {
        return nextTarget.Count == 0;
    }
}
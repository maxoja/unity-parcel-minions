using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelBot : MonoBehaviour 
{
    public Transform parcelBox;
    private int id;
    private Vector2Int target;
    [SerializeField]
    private Vector2Int current;
    [SerializeField]
    private Vector2Int nextStep;

    private Queue<Vector2Int> nextTarget;

    const float speed = 5f;

    private float r = 0;

    void Start()
    {
        StartCoroutine(Forever());
    }

    void Update()
    {
        parcelBox.gameObject.SetActive(IsSendingParcel());
    }

    IEnumerator Forever () 
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        while (true)
        {
            if (current == nextStep)
            {
                if (current == target)
                {
                    if (nextTarget == null || nextTarget.Count == 0)
                    {
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                    else
                    {
                        target = nextTarget.Dequeue();
                    }
                }
                else
                {
                    yield return StartCoroutine(BotRouter.instance.RequestNextStep(this));
                    if (nextStep == current)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            r += Time.deltaTime*speed;

            Vector2 updatedPosition;
            if (r >= 1)
            {
                r = 0;
                updatedPosition = Vector2.Lerp(current, nextStep, 1) * BoardData.gridScale;
                current = nextStep;
            }
            else
            {
                updatedPosition = Vector2.Lerp(current, nextStep, r) * BoardData.gridScale;
            }

            transform.position = updatedPosition;
            yield return null;
        }
	}

    public void Initialize(int id)
    {
        this.id = id;
        nextTarget = new Queue<Vector2Int>();

        current = new Vector2Int(id, 0);
        target = current;
        nextStep = current;
        r = 0;
        transform.position = new Vector2(current.x,current.y) * BoardData.gridScale;
    }

    public void AssignNextParcelPoint(Vector2Int assignment)
    {
        nextTarget.Enqueue(assignment);
        nextTarget.Enqueue(new Vector2Int(this.id, 0));
    }

    //private IEnumerator ActuallyAssign(Vector2Int assignment)
    //{
    //    yield return new WaitForSeconds(Random.Range(0.5f, 1));
    //}

    public bool IsFree()
    {
        if (nextStep == current && current == new Vector2Int(id, 0))
            return true;
        return false;
    }

    public Vector2Int GetNextStep()
    {
        return nextStep;
    }

    public void SetNextStep(Vector2Int nextStep)
    {
        this.nextStep = nextStep;
    }

    public Vector2Int GetTarget()
    {
        return target;
    }

    public Vector2Int GetCurrent()
    {
        return current;
    }

    public int GetId()
    {
        return id;
    }

    public bool IsSendingParcel()
    {
        if(target == new Vector2Int(id,0))
            return false;
        return true;
    }

    public int GetLoadCount()
    {
        return nextTarget.Count;
    }
}
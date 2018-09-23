using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParcelBot : MonoBehaviour 
{
    public Transform parcelBox;
    private int id;
    [SerializeField]
    private Vector2Int target;
    [SerializeField]
    private Vector2Int current;
    [SerializeField]
    private Vector2Int nextStep;

    private Queue<Vector3Int> nextTarget;
    private Text textComp;

    const float speed = 4f;

    private float r = 0;

    private void Awake()
    {
        textComp = GetComponentInChildren<Text>();
    }

    void Start()
    {
        StartCoroutine(Forever());
    }

    void Update()
    {
        parcelBox.gameObject.SetActive(IsSendingParcel());
        textComp.gameObject.SetActive(IsSendingParcel());
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
                        Vector3Int newTarget = nextTarget.Dequeue();
                        target = new Vector2Int(newTarget.x, newTarget.y);

                        if (newTarget.z != 0)
                            textComp.text = newTarget.z.ToString();
                    }
                }
                else
                {
                    yield return StartCoroutine(BotRouter.instance.RequestNextStep(this));
                    if (nextStep == current)
                    {
                        yield return new WaitForSeconds(Random.Range(0.1f,0.2f));
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
        nextTarget = new Queue<Vector3Int>();

        current = new Vector2Int(id, 0);
        target = current;
        nextStep = current;
        r = 0;
        transform.position = new Vector2(current.x,current.y) * BoardData.gridScale;
    }

    public void AssignNextParcelPoint(Vector2Int assignment, int holeId)
    {
        nextTarget.Enqueue(new Vector3Int(assignment.x,assignment.y,holeId));
        nextTarget.Enqueue(new Vector3Int(this.id, 0, 0));
        textComp.text = holeId.ToString();
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
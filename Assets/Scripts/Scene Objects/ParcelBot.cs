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
    private BatteryBar battery;

    [SerializeField]
    private Text idText, posText;
    private Queue<Vector3Int> nextTarget;
    //private Text textComp;

    const float steppingSpeed = 4f;

    private float r = 0;

    private void Awake()
    {
        idText = GetComponentInChildren<Text>();
        battery = GetComponentInChildren<BatteryBar>();
    }

    void Start()
    {
        StartCoroutine(Forever());
    }

    void Update()
    {
        //show parcel box if the bot is moving toward parcel holes
        parcelBox.gameObject.SetActive(IsSendingParcel());
        idText.gameObject.SetActive(IsSendingParcel());
        posText.text = current.x.ToString() + "-" + current.y.ToString();
    }

    IEnumerator Forever()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        while (true)
        {
            //this bot reached the next block
            //then request the new next block from BotRouter
            if (current == nextStep)
            {
                if (current == new Vector2Int(id, 0))
                    battery.StartCharging();
                else
                    battery.StopCharging();
                
                if (current == target)
                {
                    if (nextTarget == null || nextTarget.Count == 0)
                    {
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                    else
                    {
                        if (nextTarget.Count % 2 == 1)
                            yield return new WaitForSeconds(2);

                        Vector3Int newTarget = nextTarget.Dequeue();
                        target = new Vector2Int(newTarget.x, newTarget.y);

                        if (newTarget.z != 0)
                            idText.text = newTarget.z.ToString();
                    }
                }
                else
                {
                    yield return StartCoroutine(BotRouter.instance.RequestNextStep(this));

                    if (current == new Vector2Int(id, 0))
                        while (battery.LowBattery())
                            yield return new WaitForSeconds(1);
                    
                    if (nextStep == current)
                    {
                        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                    }
                    else
                    {
                        battery.DrainBattery();
                    }
                }
            }

            //ajust the value of r representing its progress toward the block
            //the value in r will be in the range of 0.0 to 1.0
            r += Time.deltaTime * steppingSpeed;

            Vector2 updatedPosition;
            if (r >= 1)
            {
                //r >= 1 means the bot should reached the target block by now

                //reset r
                r = 0;
                //set updating position to target block position
                updatedPosition = new Vector2(nextStep.x, nextStep.y) * BoardData.gridScale;
                //update current block position
                current = nextStep;
            }
            else
            {
                //calculation proper position for this bot based on its progress r
                updatedPosition = Vector2.Lerp(current, nextStep, r) * BoardData.gridScale;
            }

            //assign newly calculated position
            transform.position = updatedPosition;
            yield return null;
        }
    }

    //this will be call after a bot was created
    public void Initialize(int id)
    {
        //set id
        this.id = id;
        //initialize Queue
        nextTarget = new Queue<Vector3Int>();

        //set beginning position
        current = new Vector2Int(id, 0);

        //set initial values for calculation
        target = current;
        nextStep = current;
        r = 0;

        //assign its proper starting position
        transform.position = new Vector2(current.x, current.y) * BoardData.gridScale;
    }

    //call this function to make a bot move to a point on the board
    public void AssignNextParcelPoint(Vector2Int assignment, int holeId)
    {
        nextTarget.Enqueue(new Vector3Int(assignment.x, assignment.y, holeId));
        nextTarget.Enqueue(new Vector3Int(this.id, 0, 0));
        idText.text = holeId.ToString();
    }

    //private IEnumerator ActuallyAssign(Vector2Int assignment)
    //{
    //    yield return new WaitForSeconds(Random.Range(0.5f, 1));
    //}

    public bool IsFree()
    {
        if (nextStep == current && current == new Vector2Int(id, 0) && battery.LowBattery() == false)
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
        if (target == new Vector2Int(id, 0))
            return false;
        return true;
    }

    //check how many parcels this bot need to carry
    public int GetLoadCount()
    {
        return nextTarget.Count;
    }
}
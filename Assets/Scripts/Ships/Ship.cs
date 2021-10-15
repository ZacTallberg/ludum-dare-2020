using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{

    public float reach = 2f;
    public float speed = 1f;

    internal Vector3 moveToPos;
    public GameObject target;
    internal Coroutine currentActionCoroutine;
    internal Coroutine currentMovementCoroutine;
    public delegate void setIdleStatus(GameObject me, bool isIdle);
    public static event setIdleStatus setIdle;

    private Blob captor;

    // Start is called before the first frame update
    void Start()
    {
        setIdleState(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //deselects ship @ clickmanager
    public delegate void deselectMe(GameObject self);
    public static event deselectMe deselectSelf;

    internal abstract IEnumerator actOnObjectLoop();

    public IEnumerator moveToLocationLoop()
    {
        setIdleState(false);
        // move towards target point
        while (moveToPos != Vector3.zero && this.gameObject.transform.position != moveToPos)
        {
            Vector3 tempPos = Vector3.MoveTowards(gameObject.transform.position, moveToPos, Time.deltaTime * speed);
            gameObject.transform.position = tempPos;
            transform.LookAt(moveToPos, Vector3.back);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        moveToPos = Vector3.zero;
        currentActionCoroutine = null;
        setIdleState(true);
    }

    internal IEnumerator followObjectLoop()
    {
        bool isCloseToTarget = false;
        while (target != null)
        {
            Vector3 tempTargetPos = target.gameObject.GetComponent<Collider>().ClosestPoint(this.gameObject.transform.position);
            isCloseToTarget = isCloseTo(tempTargetPos);
            if (!isCloseToTarget)
            {
                Vector3 tempPos = Vector3.MoveTowards(gameObject.transform.position, tempTargetPos, Time.deltaTime * speed);
                gameObject.transform.position = tempPos;

                transform.LookAt(tempTargetPos, Vector3.back);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void actOn(GameObject target)
    {
        if (!isCaptured())
        {
            // Debug.Log("actOn " + target);
            this.target = target;
            moveToPos = Vector3.zero;

            // an action order cancels any current action or movement order
            if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
            if (currentMovementCoroutine != null) StopCoroutine(currentMovementCoroutine);

            currentActionCoroutine = StartCoroutine(actOnObjectLoop());
        }
        else
        {
            // Debug.Log("actOn " + target + " called, but this ship has been captured by blob " + captor);
        }
    }

    public void moveTo(Vector3 pos)
    {
        if (!isCaptured())
        {
            moveToPos = pos;

            // a move order cancels any current action or movement order
            target = null;
            if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
            if (currentMovementCoroutine != null) StopCoroutine(currentMovementCoroutine);

            currentActionCoroutine = StartCoroutine(moveToLocationLoop());
        }
        else
        {
            // Debug.Log("moveTo " + pos + " called, but this ship has been captured by blob " + captor);
        }
    }

    internal bool isCloseTo(Vector3 point)
    {
        return (
            Mathf.Abs(this.transform.position.x - point.x) < reach / 2f
            && Mathf.Abs(this.transform.position.y - point.y) < reach / 2f
        );
    }

    internal bool isInRange(Vector3 point)
    {
        return Vector3.Distance(point, this.gameObject.transform.position) <= reach;
    }

    internal void setIdleState(bool isIdle)
    {
        if (this.gameObject.name.Contains("fighter"))
        {
            return;
        }
        else
        {
            setIdle(this.gameObject, isIdle);
        }

    }

    public void getCapturedByBlob(Blob captor)
    {
        this.captor = captor;
        this.gameObject.transform.position = captor.gameObject.transform.position;
        this.gameObject.transform.SetParent(captor.gameObject.transform);
        setIdleState(false);

        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
        }
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
    }

    public void releasedByBlob(Blob captor)
    {
        if (this.captor == captor)
        {
            this.captor = null;
            this.gameObject.transform.SetParent(null);
            setIdleState(true);
        }
    }

    public bool isCaptured()
    {
        return captor != null;
    }

}
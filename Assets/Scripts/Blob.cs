using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{

    public int health = 50;
    public float speed = 1.5f;
    public float reach = 0.5f;

    private float targetElectionDelay = 2.5f;
    public Ship target;
    private bool shipIsCaptured;
    private BlackHole targetBlackHole;

    private Coroutine currentAction;

    void OnEnable()
    {
        BlackHole bh = GameObject.Find("BlackHole").GetComponent<BlackHole>();
        bh.addBlob(this);
    }

    void OnDisable()
    {
        BlackHole bh = GameObject.Find("BlackHole").GetComponent<BlackHole>();
        bh.removeBlob(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAction = StartCoroutine(pursueTargetLoop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator pursueTargetLoop()
    {
        while (!shipIsCaptured)
        {
            if (target == null)
            {
                electTarget();
            }

            if (target != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.gameObject.transform.position, Time.deltaTime * speed);
                transform.LookAt(target.gameObject.transform.position, Vector3.back);
            }

            yield return new WaitForSeconds(Time.deltaTime);

            if (target != null)
            {
                shipIsCaptured = isCloseTo(target.gameObject.transform.position);
            }
        }

        // we have captured a ship - drag it back to the black hole
        target.gameObject.GetComponent<SphereCollider>().enabled = false;
        target.gameObject.GetComponent<ToggleOutline>().toggleOutlineOff();
        target.getCapturedByBlob(this);
        currentAction = StartCoroutine(dragTargetToBlackHoleLoop());
    }

    public IEnumerator dragTargetToBlackHoleLoop()
    {
        while (target != null)
        {
            if (targetBlackHole == null)
            {
                electTargetBlackHole();
            }

            if (targetBlackHole != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetBlackHole.gameObject.transform.position, Time.deltaTime * speed);
                transform.LookAt(target.gameObject.transform.position, Vector3.back);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        shipIsCaptured = false;
        yield return new WaitForSeconds(targetElectionDelay);

        currentAction = StartCoroutine(pursueTargetLoop());
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (target != null)
        {
            target.gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    private void electTarget()
    {
        List<Ship> ships = new List<Ship>(FindObjectsOfType<Ship>());
        ships.Sort((ship1, ship2) => (Mathf.RoundToInt(
            Vector3.Distance(ship1.gameObject.transform.position, this.gameObject.transform.position)
            - Vector3.Distance(ship2.gameObject.transform.position, this.gameObject.transform.position)
        )));
        while (target == null && ships.Count > 0)
        {
            foreach (Ship ship in ships)
            {
                if (Random.value < 0.5f)
                {
                    target = ship;
                    break;
                }
            }
        }
    }

    private void electTargetBlackHole()
    {
        List<BlackHole> blackHoles = new List<BlackHole>(FindObjectsOfType<BlackHole>());
        blackHoles.Sort((bh1, bh2) => (Mathf.RoundToInt(
            Vector3.Distance(bh1.gameObject.transform.position, this.gameObject.transform.position)
            - Vector3.Distance(bh2.gameObject.transform.position, this.gameObject.transform.position)
        )));
        if (blackHoles.Count > 0)
        {
            targetBlackHole = blackHoles[0];
        }
    }

    internal bool isCloseTo(Vector3 point)
    {
        return (
            Mathf.Abs(this.transform.position.x - point.x) < reach
            && Mathf.Abs(this.transform.position.y - point.y) < reach
        );
    }

    public void takeDamage(int damageDealt)
    {
        health -= damageDealt;

        if (health <= 0)
        {
            if (target != null)
                target.releasedByBlob(this);
            if (currentAction != null)
            {
                StopCoroutine(currentAction);
            }
            Destroy(this.gameObject);
        }
    }
}

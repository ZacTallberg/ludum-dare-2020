using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterShip : Ship
{

    public int attackPower = 5;

    public float fireCooldown = 1.5f;
    public float fireRange = 50f;

    private Laser laser;

    override internal IEnumerator actOnObjectLoop()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        currentMovementCoroutine = StartCoroutine(followObjectLoop());

        yield return null;
    }

    private IEnumerator fireAtTargetsLoop()
    {
        yield return new WaitForSeconds(.5f);

        while (true)
        {
            if (target != null && target.GetComponent<Blob>() && isInRange(target.gameObject.transform.position))
            {
                // fire at target specifically
                laser.shootAt(target.gameObject.transform.position);
                target.GetComponent<Blob>().takeDamage(attackPower);
                yield return new WaitForSeconds(fireCooldown);
            }
            else
            {
                // fire at anything in range
                Blob closestInRange = electTarget();
                Debug.Log("Next closest target in range: " + closestInRange);

                if (closestInRange != null)
                {
                    Debug.Log("Firing at " + closestInRange);
                    laser.shootAt(closestInRange.gameObject.transform.position);
                    closestInRange.takeDamage(attackPower);
                    yield return new WaitForSeconds(fireCooldown);
                }

                // move towards the target if there is one
                if (target != null)
                {
                    // TODO approach target on a curved path or something?
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        laser = gameObject.GetComponent<Laser>();

        StartCoroutine(fireAtTargetsLoop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool canAimAt(GameObject target)
    {
        RaycastHit objectHit;
        bool result = Physics.Raycast(this.gameObject.transform.position, target.gameObject.transform.position, out objectHit, fireRange);
        return result && objectHit.transform.gameObject == target.gameObject;
    }

    private Blob electTarget()
    {
        if (GameObject.Find("BlackHole"))
        {
            List<Blob> blobs = GameObject.Find("BlackHole").GetComponent<BlackHole>().getBlobs();


            blobs = blobs.FindAll((blob) => (
                Vector3.Distance(blob.gameObject.transform.position, this.gameObject.transform.position) <= reach
            ));

            if (blobs != null && blobs.Count > 0)
            {
                blobs.Sort((blob1, blob2) => (Mathf.RoundToInt(
                    Vector3.Distance(blob1.gameObject.transform.position, this.gameObject.transform.position)
                    - Vector3.Distance(blob2.gameObject.transform.position, this.gameObject.transform.position)
                )));

                return blobs[0];
            }


        }
        return null;
    }
}

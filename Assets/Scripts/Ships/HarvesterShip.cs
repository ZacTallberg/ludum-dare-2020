using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterShip : Ship
{

    public int miningStrength = 1;
    public float miningCooldown = 0.1f;

    override internal IEnumerator actOnObjectLoop()
    {
        setIdleState(false);

        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        currentMovementCoroutine = StartCoroutine(followObjectLoop());

        while (target != null)
        {
            if (target.GetComponent<Antimatter>())
            {
                Debug.Log(gameObject.transform.GetChild(0).GetComponent<Antimatter>());
                if (Vector3.Distance(target.gameObject.GetComponent<Collider>().ClosestPoint(this.gameObject.transform.position), this.gameObject.transform.position) <= reach)
                {

                    target.gameObject.transform.SetParent(this.gameObject.transform);
                    target.GetComponent<SphereCollider>().enabled = false;
                    Vector3 tempTargetPos = target.transform.position;
                    tempTargetPos = new Vector3(0, 0, -1.5f);
                    target.gameObject.transform.localPosition = tempTargetPos;

                    if (currentMovementCoroutine != null)
                    {
                        StopCoroutine(currentMovementCoroutine);
                    }

                    target = null;
                }
                else
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            else if (target.GetComponent<BlackHole>())
            {
                moveTo(target.gameObject.transform.position);
            }
            else
            {
                if (isInRange(target.gameObject.GetComponent<Collider>().ClosestPoint(this.gameObject.transform.position)))
                {
                    target.GetComponent<Asteroid>().mineMatter(miningStrength);
                    yield return new WaitForSeconds(miningCooldown);
                }
                else
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
        }

        setIdleState(true);
    }

}
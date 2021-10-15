using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Orbital : MonoBehaviour
{
    public float rotateSpeed = 1f;

    public float rotateDistance;
    public float theta = 0f;
    public Orbitable target;
    private Func<float, float> xPosFunction = Mathf.Sin;
    private Func<float, float> yPosFunction = Mathf.Cos;
    private Orbitable[] orbitables;
    public bool randomSpeed;
    public bool randomStartPos;

    // Start is called before the first frame update
    void Start()
    {
        if (randomSpeed == true)
        {

            if (this.gameObject.tag == "Moon")
            {
                rotateSpeed = UnityEngine.Random.Range(0.5f, 0.8f);
            }
            else
            {
                rotateSpeed = UnityEngine.Random.Range(0.05f, 0.12f);
            }
        }
        if (randomStartPos == true)
        {
            theta = UnityEngine.Random.Range(0f, 6.2f);
            if (this.gameObject.name == "Earth")
            {
                theta = 5f;
            }
        }
        if (gameObject.name == "Earth")
        {
            calculateEarthDistance();
        }
        if (this.gameObject.tag == "Moon")
        {
            orbitables = FindObjectsOfType<Orbitable>();
        }
        else
        {
            orbitables =
                (from o in FindObjectsOfType<Orbitable>()
                 where o.gameObject.tag != "Planet"
                 select o).ToArray<Orbitable>();
        }

        checkOrbitables();
    }

    public delegate void sendDistance(float earthOrbit);
    public static event sendDistance sendNow;

    public void calculateEarthDistance()
    {
        GameObject sunObj = GameObject.Find("Sun");
        GameObject blackObj = GameObject.Find("BlackHole");
        float totalDist = Vector3.Distance(sunObj.transform.position, blackObj.GetComponent<Collider>().ClosestPoint(sunObj.transform.position));
        float orbitRadius = Vector3.Distance(sunObj.transform.position, this.gameObject.transform.position);
        orbitRadius += gameObject.GetComponent<SphereCollider>().radius;
        sendNow(orbitRadius);

    }
    // Update is called once per frame
    void Update()
    {
        // check if we need to switch the object we're orbiting
        // checkOrbitables();

        // adjust the angle to calculate new position from
        theta += rotateSpeed * Time.deltaTime;

        // move to new position based on angle
        Vector3 newPos = new Vector3(xPosFunction(theta), yPosFunction(theta), 0f) * rotateDistance;
        if (target != null)
        {
            this.gameObject.transform.position = target.gameObject.transform.position + newPos;
        }
        //Vector3.Lerp(gameObject.transform.position, target.gameObject.transform.position + newPos, Time.deltaTime);
    }

    void checkOrbitables()
    {
        // get closest orbitable
        Orbitable closest = null;
        float closestDistance = float.MaxValue;

        foreach (Orbitable orbitable in orbitables)
        {
            if (orbitable == null)
            {
                continue;
            }
            float delta = Vector3.Distance(
                orbitable.gameObject.GetComponent<Collider>().ClosestPoint(this.gameObject.transform.position),
                this.gameObject.transform.position);
            if (delta < closestDistance)
            {
                closest = orbitable;
                closestDistance = delta;
            }
        }

        // if the orbitable changed, we should flip the rotation direction - counterclockwise to clockwise and vice versa
        if (target != closest)
        {
            Func<float, float> temp = xPosFunction;
            xPosFunction = yPosFunction;
            yPosFunction = temp;

            float newRadius = Vector3.Distance(closest.gameObject.transform.position, this.gameObject.transform.position);

            if (target != null)
            {
                // also should adjust the rotation speed, because the orbital radius may have changed.
                float oldRadius = rotateDistance;
                rotateSpeed *= oldRadius / newRadius;
            }

            target = closest;
            rotateDistance = newRadius;
        }
    }
}

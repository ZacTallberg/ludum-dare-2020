using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    public bool useRandomVals = false;

    public float speed;
    public float x;
    public float y;
    public float z;
    // Start is called before the first frame update
    void Start()
    {
        if (useRandomVals)
        {
            x = Random.value * 10f - 5f;
            y = Random.value * 10f - 5f;
            z = Random.value * 10f - 5f;
            speed = Random.value * 10f - 5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * x * speed, Time.deltaTime * y * speed, Time.deltaTime * z * speed);
    }
}

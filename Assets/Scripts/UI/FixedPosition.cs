using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosition : MonoBehaviour
{

    public GameObject self;
    public float updown = 3;
    public float leftright = 25;

    // Start is called before the first frame update
    void Start()
    {
        if (self == null) self = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

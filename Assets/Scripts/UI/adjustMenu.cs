using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustMenu : UIRadialLayout
{
    public List<Transform> children;
    //This is the distance in radians between the cameraLocation and menuLocation (b/t 0 and ~1.56)
    public bool outsideMin;
    public float distanceFrom;
    public int currentTile;
    public Canvas selfCanvas;
    Transform hexaT;
    //gameobject that lives on the surface of the planet to find tile between planet and camera
    public Transform planetSurface;
    //actual location of the object that's used to find the tile between the planet and camera
    public Vector3 cameraLocation;
    //location of (this) menu object
    public Vector3 menuLocation;
    //Int of the tile the menu is centered on
    public int tileAtMenuLoc;
    //maximum distance the camera can move from the menu before it disappears
    public float maxDistance;
    //distance the camera can move before it starts becoming transparent
    public float minDistance;
    //Floats to control fade in time on UI
    public float fadeInTime;
    public float fadeOutTime;
    Vector3 test;

    //Starts dynamicMenu actions
    public delegate void checkDisableMenu();
    public static event checkDisableMenu disableMenu;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    //If you click into space, disable the menu
    //Uses IT_Gesture.onShortTapE event to trigger function call
    void toggleMenuActiveOnNull(Vector2 position)
    {
        if (IT_Utility.GetHovered3DObject(position, Camera.main) == null)
        {
            selfCanvas.enabled = false;
        }
    }
    void CanvasFlip()
    {
        selfCanvas.enabled = !selfCanvas.enabled;
    }

    public IEnumerator killMenu(float waitTime)
    {
        yield return new WaitForSeconds(0.1f);
        selfCanvas.enabled = false;
        //set the tile int to a not-possible value?
        tileAtMenuLoc = -1;
        yield return null;
    }

    public IEnumerator killMenu()
    {
        //yield return StartCoroutine(fadeOutMenu());
        selfCanvas.enabled = false;
        tileAtMenuLoc = -1;
        yield return null;
    }
    public delegate void fadeButton(float value);
    public static event fadeButton fadeNow;

    // void adjustFadeOnDistance()
    // {
    //     //calculates spherical distance between camera point @ surface and menu, between 0 and 1.56
    //     distanceFrom = try2(cameraLocation, menuLocation);
    //     //NEED TO PUT IN SPECIAL HELPER METHODS FOR MAKING MENU PRETTY
    //     if (distanceFrom > maxDistance && selfCanvas.enabled == true)
    //     {
    //         StartCoroutine(killMenu());
    //     }
    //     if (distanceFrom < maxDistance && distanceFrom > minDistance)
    //     {
    //         if (outsideMin == true)
    //         {
    //             float distance = distanceFrom - minDistance;
    //             //calculate percentage past the min distance
    //             float fade = (100 - (100 * distance) / (maxDistance - minDistance)) / 100;
    //             fadeNow(fade);
    //         }
    //         outsideMin = true;
    //         //Debug.Log("yep");

    //     }
    //     else
    //     {
    //         if (outsideMin == true)
    //         {
    //             fadeNow(1);
    //         }
    //         outsideMin = false;
    //     }
    // }

    void Update()
    {


        //get direction between menu
        Vector3 camPos = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(camPos, transform.up);



        if (menuLocation != transform.position) menuLocation = transform.position;

        // if(selfCanvas.enabled == true)
        // {
        //     adjustFadeOnDistance();
        // }
        // else{
        //     distanceFrom = 0;
        // } 


    }
    float try2(Vector3 first, Vector3 second)
    {
        //find angle between the two points, transform it to radians
        float angle = Vector3.Angle(first, second) * Mathf.Deg2Rad;
        //finds circular distance in radians
        float distance = angle * hexaT.GetComponent<SphereCollider>().radius;
        return distance;
    }


}

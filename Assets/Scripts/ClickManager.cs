using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{

    public GameObject lastLeftClickedOn;
    public GameObject lastRightClickedOn;
    public List<Transform> selectedObjects;

    //Connects to CameraController, forces the camera to follow this game object
    public delegate void camFocusThisObject(GameObject followMe);
    public static event camFocusThisObject focusCam;

    private void OnEnable()
    {
        IT_Gesture.onMouse1DownE += getThisLeftClick;
        IT_Gesture.onMouse2DownE += getThisRightClick;
        ToggleOutline.selectMe += selectTheseObjects;
        ToggleOutline.deselectMe += deselectTheseObjects;
    }
    private void OnDisable()
    {
        IT_Gesture.onMouse1DownE -= getThisLeftClick;
        IT_Gesture.onMouse2DownE -= getThisRightClick;
        ToggleOutline.selectMe -= selectTheseObjects;
        ToggleOutline.deselectMe -= deselectTheseObjects;
    }

    //Connects to all ToggleOutline.cs, clears _all_ outlines of every object
    public delegate void clearAllOutlines();
    public static event clearAllOutlines clearOutlines;

    private void selectTheseObjects(Transform thisOne)
    {
        if (!selectedObjects.Contains(thisOne))
        {
            selectedObjects.Add(thisOne);
        }
    }
    private void deselectTheseObjects(Transform thisOne)
    {
        if (selectedObjects.Contains(thisOne))
        {
            selectedObjects.Remove(thisOne);
        }
    }
    private void checkForObjects(GameObject thisOne)
    {
        if (thisOne.name == "Earth")
        {
            focusCam(thisOne);
        }
        if (thisOne.tag == "Planet")
        {
            focusCam(thisOne);
        }
    }
    public void getThisLeftClick(Vector2 pos)
    {
        try
        {
            GameObject newClickedOn = IT_Utility.GetHovered3DObject(pos, Camera.main);
            GameObject uiClickedOn = IT_Utility.GetHoveredUIElement(pos);

            if (newClickedOn != lastLeftClickedOn && uiClickedOn == null)
            {
                clearOutlines();
                lastLeftClickedOn = newClickedOn;
            }
            if (lastLeftClickedOn != null)
            {
                checkForObjects(lastLeftClickedOn);
                lastLeftClickedOn.GetComponent<ToggleOutline>().toggleOutlineOn();
            }
            if (lastLeftClickedOn == null)
            {
            }
        }
        catch (NullReferenceException e) { }
    }

    public void getThisRightClick(Vector2 pos)
    {
        try
        {
            lastRightClickedOn = IT_Utility.GetHovered3DObject(pos, Camera.main);
            if (lastLeftClickedOn != null)
            {
                Ship selectedShip = lastLeftClickedOn.GetComponent<Ship>();
                if (selectedShip != null)
                {
                    // we are giving a ship an action
                    if (lastRightClickedOn != null)
                    {
                        ShipWorkable targetOfAction = lastRightClickedOn.GetComponent<ShipWorkable>();
                        if (targetOfAction != null)
                        {
                            selectedShip.actOn(targetOfAction.gameObject);
                        }
                    }
                    else
                    {
                        // this is a move order
                        Vector3 movePoint = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, -Camera.main.gameObject.transform.position.z));
                        movePoint.z = 0f;
                        selectedShip.moveTo(movePoint);
                    }
                }
            }
        }
        catch (NullReferenceException e) { }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
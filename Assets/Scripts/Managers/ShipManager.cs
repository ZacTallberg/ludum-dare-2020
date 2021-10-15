using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{

    public List<Transform> idleShips;
    public CameraController cameraController;
    public ClickManager clickManager;


    //Connects to CameraController to move camera to an idleship
    public delegate void idleShipSelect(Transform thisShip);
    public static event idleShipSelect idleShip;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Ship.setIdle += checkIdleList;
        BlackHole.setIdle += checkIdleList;
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Ship.setIdle -= checkIdleList;
        BlackHole.setIdle -= checkIdleList;
    }

    void checkIdleList(GameObject shipObj, bool isIdle)
    {
        if (isIdle)
        {
            if (!idleShips.Contains(shipObj.transform))
            {
                idleShips.Add(shipObj.transform);
            }
        }
        else
        {
            if (idleShips.Contains(shipObj.transform))
            {
                idleShips.Remove(shipObj.transform);
            }
        }
    }

    //FIX THE REMOVAL OF SELECTED OBJECTS FROM THIS LIST 
    public void popIdleShip()
    {
        if (cameraController.currentCamMove != null)
        {
            return;
        }
        Debug.Log("Moving to idle ship now");
        if (idleShips.Count > 0)
        {
            List<Transform> tempHolder = new List<Transform>(idleShips);
            List<Transform> tempSelected = new List<Transform>(clickManager.selectedObjects);
            if (clickManager.selectedObjects.Count > 0)
            {
                foreach (Transform ship in clickManager.selectedObjects)
                {
                    if (ship == null)
                    {
                        clickManager.selectedObjects.Remove(ship);
                        popIdleShip();
                        return;
                    }
                    if (tempHolder.Contains(ship))
                    {
                        tempHolder.Remove(ship);
                    }
                }
            }
            if (tempHolder.Count > 0)
            {
                foreach (Transform ship in tempSelected)
                {
                    ship.GetComponent<ToggleOutline>().toggleOutlineOff();
                }
                Transform firstship = tempHolder[Random.Range(0, tempHolder.Count)];
                idleShip(firstship);
            }
            else
            {
                Debug.Log("no idle ships");
            }


        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cameraController == null) cameraController = Camera.main.GetComponent<CameraController>();
        if (clickManager == null) clickManager = gameObject.GetComponent<ClickManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

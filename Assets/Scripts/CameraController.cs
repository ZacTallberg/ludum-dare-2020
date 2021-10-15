using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public bool moveBool = true;
    public Coroutine moveCoroutine;
    public float dampener = 50f;
    public float scrollDampener = 0.6f;
    public float scrollSpeed = 1f;
    private float scrollAccel;
    public float freedom = 0.2f;
    public float focusSpeed = 2f;
    public float cameraSpeed = 2f;
    public float moveSpeed = 1f;
    public Coroutine currentCamMove;
    public GameObject cameraTarget;
    public GameObject cameraObject;
    private Vector2 desiredOffset;
    private float tempX;
    private float tempY;
    public ClickManager clickManager;
    public Canvas popUI;



    // Start is called before the first frame update
    void Start()
    {
        if (popUI == null) popUI = GameObject.Find("Earth").GetComponentInChildren<Canvas>();
        if (cameraTarget == null) cameraTarget = GameObject.Find("cameraTarget");
        if (cameraObject == null) cameraObject = gameObject;
        if (clickManager == null) clickManager = GameObject.Find("Managers").GetComponent<ClickManager>();
        StartCoroutine(moveTarget());
    }

    void OnEnable()
    {
        ShipManager.idleShip += moveCameraNow;
        ClickManager.focusCam += focusCameraNow;
    }

    void OnDisable()
    {
        ShipManager.idleShip -= moveCameraNow;
        ClickManager.focusCam -= focusCameraNow;
    }

    public void focusCameraNow(GameObject focusedCamera)
    {
        StartCoroutine(focusCamCoroutine(focusedCamera));
    }

    public IEnumerator focusCamCoroutine(GameObject focus)
    {
        if (focus.name == "Earth")
        {
            popUI.enabled = true;
        }
        if (focus.name != "Earth")
        {
            focus.GetComponent<ToggleOutline>().toggleOutlineOn();
        }
        clickManager.lastLeftClickedOn = focus;
        while (clickManager.lastLeftClickedOn == focus)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            {
                clickManager.lastLeftClickedOn = null;
            }
            freedom = 0f;
            cameraSpeed = 50f;
            var move = new Vector3(focus.transform.position.x, focus.transform.position.y, 0);
            cameraTarget.transform.position = move;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (focus.name == "Earth")
        {
            popUI.enabled = false;
        }
        freedom = 1f;
        cameraSpeed = 10f;
    }
    public void SetTargetObjectPosition()
    {
        if (cameraTarget.transform.position.x < (cameraObject.transform.position.x - freedom))
        {
            desiredOffset.x = cameraTarget.transform.position.x + freedom;
        }
        else if (cameraTarget.transform.position.x > (cameraObject.transform.position.x + freedom))
        {
            desiredOffset.x = cameraTarget.transform.position.x - freedom;
        }
        if (cameraTarget.transform.position.y < (cameraObject.transform.position.y - freedom))
        {
            desiredOffset.y = cameraTarget.transform.position.y + freedom;
        }
        else if (cameraTarget.transform.position.y > (cameraObject.transform.position.y + freedom))
        {
            desiredOffset.y = cameraTarget.transform.position.y - freedom;
        }

    }
    void Update()
    {
        if (cameraTarget != null)
        {
            SetTargetObjectPosition();
            if (cameraObject.transform.position.x != cameraTarget.transform.position.x + freedom)
            {

                tempX = cameraObject.transform.position.x;
                tempX = Mathf.Lerp(tempX, desiredOffset.x, Time.deltaTime * cameraSpeed * dampener);

                //CameraObject.transform.position = new Vector3 (tempX, cameraObject.transform.position.y, cameraObject.transform.position.z);
            }
            if (cameraObject.transform.position.y != cameraTarget.transform.position.y + freedom)
            {
                tempY = cameraObject.transform.position.y;
                tempY = Mathf.Lerp(tempY, desiredOffset.y, Time.deltaTime * cameraSpeed * dampener);
                //cameraObject.transform.position = new Vector3 (cameraObject.transform.position.x, tempY, cameraObject.transform.position.z);
            }


            cameraObject.transform.localPosition = new Vector3(tempX, tempY, cameraObject.transform.localPosition.z);
            //activeObj.transform.localPosition = new Vector3 (tempX + 0.5f, tempY, tempZ);
        }
    }
    public IEnumerator moveTarget()
    {


        while (moveBool == true)
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            scrollAccel += -wheel;

            var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            cameraTarget.transform.position += move * moveSpeed * Time.deltaTime;

            if (scrollAccel != 0)
            {
                scrollAccel = Mathf.Clamp(scrollAccel, -0.1f, 0.1f);
                Vector3 transPos = new Vector3(0, 0, transform.position.z * scrollAccel * scrollSpeed);
                Vector3 tempPos = transform.position + transPos;
                transform.position = tempPos;
                scrollAccel *= scrollDampener;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

    }

    public void moveCameraNow(Transform shipLoc)
    {
        shipLoc.GetComponent<ToggleOutline>().toggleOutlineOn();
        clickManager.lastLeftClickedOn = shipLoc.gameObject;
        currentCamMove = StartCoroutine(moveCameraToShip(shipLoc));
    }
    public IEnumerator moveCameraToShip(Transform shipLocation)
    {

        while ((
                new Vector2(cameraTarget.transform.position.x, cameraTarget.transform.position.y)
                - new Vector2(shipLocation.transform.position.x, shipLocation.transform.position.y)
            ).magnitude >= freedom)
        {
            Vector3 currentPosition = cameraTarget.transform.position;
            Vector3 targetPosition = new Vector3(shipLocation.position.x, shipLocation.position.y, cameraTarget.transform.position.z);

            cameraTarget.transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * focusSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        currentCamMove = null;


    }
    // Update is called once per frame

}

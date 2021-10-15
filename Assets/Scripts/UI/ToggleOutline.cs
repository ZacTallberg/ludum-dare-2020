using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOutline : MonoBehaviour
{
    public Outline selfOutline;
    public bool toggled;
    // Start is called before the first frame update
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    /// 
    void OnEnable()
    {
        ClickManager.clearOutlines += toggleOutlineOff;
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        ClickManager.clearOutlines -= toggleOutlineOff;
    }
    void Start()
    {
        if (selfOutline == null) selfOutline = gameObject.GetComponent<Outline>();
        selfOutline.enabled = false;
    }
    public delegate void selectedThisOne(Transform me);
    public static event selectedThisOne selectMe;
    public delegate void deselectThisOne(Transform me);
    public static event deselectThisOne deselectMe;
    public void toggleOutlineOn()
    {
        selectMe(this.gameObject.transform);
        selfOutline.enabled = true;
        toggled = true;
    }

    public void toggleOutlineOff()
    {
        deselectMe(this.gameObject.transform);
        selfOutline.enabled = false;
        toggled = false;
    }
    void OnMouseOver()
    {
        if (!toggled)
        {
            selfOutline.enabled = true;
        }
    }
    void OnMouseExit()
    {
        if (!toggled)
        {
            selfOutline.enabled = false;
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}

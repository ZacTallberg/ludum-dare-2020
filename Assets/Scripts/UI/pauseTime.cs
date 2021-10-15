using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseTime : MonoBehaviour
{
    public Image tutorialPane;
    public bool firstTimeclick;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void restartTime()
    {
        Time.timeScale = 1f;
    }
    // Update is called once per frame
    void Update()
    {

    }
}

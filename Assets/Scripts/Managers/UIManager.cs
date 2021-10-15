using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public int totalMinerals;
    public int totalEnergy;
    public Text mineralLabel;
    public Text energyLabel;
    public Text percentLabel;
    public GameObject instructionsPanel;
    public GameObject transBackground;
    public GameObject transEnd;
    public GameObject idleIcon;
    private float earthOrbitDist;
    private BlackHole blackHole;
    private ShipManager shipManager;
    public GameObject UIParent;
    public GameObject victory;
    public GameObject defeat;

    void Start()
    {

        if (UIParent == null) UIParent = GameObject.Find("UI");
        if (blackHole == null) blackHole = GameObject.Find("BlackHole").GetComponent<BlackHole>();
        if (shipManager == null) shipManager = gameObject.GetComponent<ShipManager>();
        foreach (Transform child in UIParent.GetComponentsInChildren<Transform>())
        {
            if (child.name == "minerallabel")
            {
                mineralLabel = child.GetComponent<Text>();
            }
            if (child.name == "energylabel")
            {
                energyLabel = child.GetComponent<Text>();
            }
            if (child.name == "percentlabel")
            {
                percentLabel = child.GetComponent<Text>();
            }
            if (child.name == "idleships")
            {
                idleIcon = child.gameObject;
            }
            if (child.name == "instructions_pane")
            {
                instructionsPanel = child.gameObject;
            }
            if (child.name == "transparent")
            {
                transBackground = child.gameObject;
            }
            if (child.name == "defeat" && defeat == null)
            {
                defeat = child.gameObject;
            }
            if (child.name == "victory" && victory == null)
            {
                victory = child.gameObject;
            }
            if (child.name == "transparent_end")
            {
                transEnd = child.gameObject;
            }
        }
    }

    public void endGame()
    {
        Application.Quit();
    }

    public void popVictory()
    {
        victory.SetActive(true);
        transEnd.SetActive(!transEnd.activeSelf);
        Time.timeScale = 0f;
    }
    public void popDefeat()
    {
        defeat.SetActive(true);
        transEnd.SetActive(!transEnd.activeSelf);
        Time.timeScale = 0f;
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Orbital.sendNow += captureDistance;
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Orbital.sendNow -= captureDistance;
    }

    public void toggleInstructionsMenu()
    {
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
        transBackground.SetActive(!transBackground.activeSelf);
    }

    void captureDistance(float dist)
    {
        earthOrbitDist = dist;
    }

    float calculatePercentToDeath()
    {

        float percent = (blackHole.sunToBlack - earthOrbitDist) / (blackHole.initialDist - earthOrbitDist);
        percent *= 100;
        percent = 100 - percent;
        percent = Mathf.Clamp(percent, 1, 100);
        return percent;
    }
    public void adjustMineralTotal(int amount)
    {
        totalMinerals += amount;
    }

    public void adjustEnergyTotal(int amount)
    {
        totalEnergy += amount;
    }

    // Update is called once per frame
    void Update()
    {
        float rounded = Mathf.Round((calculatePercentToDeath() * 100f) / 100f);
        percentLabel.text = rounded.ToString() + "%";
        mineralLabel.text = totalMinerals.ToString();
        energyLabel.text = totalEnergy.ToString();
        if (shipManager.idleShips.Count > 0)
        {
            idleIcon.SetActive(true);
        }
        else
        {
            idleIcon.SetActive(false);
        }
    }
}

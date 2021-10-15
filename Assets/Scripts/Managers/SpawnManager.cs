using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int harvesterCost;
    public int fighterCost;
    public int antimatterCost;
    public int mineralToEnergyCost;
    public UIManager uiManager;
    private int mineralPool;
    private int energyPool;
    public int energyGen;
    public GameObject earth;


    // Start is called before the first frame update
    void Start()
    {
        if (uiManager == null) uiManager = gameObject.GetComponent<UIManager>();
        if (earth == null) earth = GameObject.Find("Earth");
    }

    public void spawnHarvester()
    {
        if (mineralPool >= harvesterCost)
        {
            uiManager.adjustMineralTotal(-harvesterCost);
            GameObject newShip = Instantiate(Resources.Load<GameObject>("Models/harvester_ship"));
            newShip.transform.position = new Vector3(earth.transform.position.x + 1, earth.transform.position.y + 1, 0);
        }
    }
    public void spawnFighter()
    {
        if (mineralPool >= fighterCost)
        {
            uiManager.adjustMineralTotal(-fighterCost);
            GameObject newShip = Instantiate(Resources.Load<GameObject>("Models/fighter_ship"));
            newShip.transform.position = new Vector3(earth.transform.position.x + 1, earth.transform.position.y + 1, 0);
        }
    }

    public void spawnAntimatter()
    {
        if (energyPool >= antimatterCost)
        {
            uiManager.adjustEnergyTotal(-antimatterCost);
            GameObject newAntimatter = Instantiate(Resources.Load<GameObject>("Models/antimatter"));
            newAntimatter.transform.position = new Vector3(earth.transform.position.x + 1, earth.transform.position.y + 1, 0);
        }
    }

    public void createEnergy()
    {
        if (mineralPool >= mineralToEnergyCost)
        {
            uiManager.adjustMineralTotal(-mineralToEnergyCost);
            uiManager.adjustEnergyTotal(energyGen);
        }
    }
    // Update is called once per frame
    void Update()
    {
        mineralPool = uiManager.totalMinerals;
        energyPool = uiManager.totalEnergy;
    }
}

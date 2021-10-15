using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject victory;
    public GameObject defeat;
    public GameObject blobPrefab;

    public float growthRate = 0.05f;
    private GameObject sunObj;
    public float sunToBlack;
    public float initialDist;
    public UIManager uiManager;

    public int blobSpawnRateSeconds = 60;
    public int numberOfBlobsToSpawn = 1;
    public int linearIncreaseToExponentialSpawnThreshold = 30;
    private int increaseSpawnCountBy = 1;

    public ParticleSystem drain;
    public ParticleSystem ray1;
    public ParticleSystem ray2;
    public Vector3 selfSize;

    public delegate void setIdleStatus(GameObject me, bool isIdle);
    public static event setIdleStatus setIdle;

    private List<Blob> blobs = new List<Blob>();

    private Coroutine blobSpawnerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (sunObj == null) sunObj = GameObject.Find("Sun");
        initialDist = Vector3.Distance(sunObj.transform.position, gameObject.GetComponent<Collider>().ClosestPoint(sunObj.transform.position));
        drain = transform.Find("drain").GetComponent<ParticleSystem>();
        ray1 = transform.Find("drain").transform.Find("ray_1").GetComponent<ParticleSystem>();
        ray2 = transform.Find("drain").transform.Find("ray_2").GetComponent<ParticleSystem>();
        if (uiManager == null) uiManager = GameObject.Find("Managers").GetComponent<UIManager>();
        blobSpawnerCoroutine = StartCoroutine(blobSpawnerLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (sunObj == null)
        {
            return;
        }
        else
        {
            sunToBlack = Vector3.Distance(sunObj.transform.position, gameObject.GetComponent<Collider>().ClosestPoint(sunObj.transform.position));
        }

        this.gameObject.transform.localScale += new Vector3(growthRate * Time.deltaTime, 0, growthRate * Time.deltaTime);
        drain.startSize = drain.startSize + growthRate * Time.deltaTime;
        ray1.startSize = ray1.startSize + growthRate * Time.deltaTime;
        ray2.startSize = ray2.startSize + growthRate * Time.deltaTime;
        selfSize = this.gameObject.transform.localScale;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Blob>() == null)
        {
            if (other.gameObject.GetComponent<HarvesterShip>())
            {
                setIdle(other.gameObject, false);
                if (other.gameObject.transform.childCount > 1)
                {
                    if (other.gameObject.transform.GetChild(1).GetComponent<Antimatter>())
                    {
                        Debug.Log("worked");
                        this.gameObject.transform.localScale -= new Vector3(5f, 0, 5f);
                        if (this.gameObject.transform.localScale.x < 0)
                        {
                            // TODO: victory!!!
                            Destroy(this.gameObject);
                            uiManager.popVictory();
                        }
                    }
                }
            }

            if (other.gameObject.tag == "Planet")
            {
                numberOfBlobsToSpawn += 6;
            }
            if (other.gameObject.name == "Earth")
            {
                // TODO: lose condition :(
                uiManager.popDefeat();
            }
            Destroy(other.gameObject);
        }

    }

    public void addBlob(Blob b)
    {
        blobs.Add(b);
    }

    public void removeBlob(Blob b)
    {
        blobs.Remove(b);
    }

    public List<Blob> getBlobs()
    {
        return new List<Blob>(blobs);
    }

    private IEnumerator blobSpawnerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(blobSpawnRateSeconds);

            for (int i = 0; i < numberOfBlobsToSpawn; i++)
            {
                float xNoise = Random.value * 10f - 5f;
                float yNoise = Random.value * 10f - 5f;
                Vector3 spawnAt = new Vector3(this.gameObject.transform.position.x + xNoise, this.gameObject.transform.position.y + yNoise, 0f);
                Instantiate(blobPrefab, spawnAt, Quaternion.identity);
            }

            if (numberOfBlobsToSpawn <= linearIncreaseToExponentialSpawnThreshold)
            {
                numberOfBlobsToSpawn += increaseSpawnCountBy++;
            }
            else
            {
                numberOfBlobsToSpawn += increaseSpawnCountBy;
                increaseSpawnCountBy *= increaseSpawnCountBy;
            }
        }
    }
}

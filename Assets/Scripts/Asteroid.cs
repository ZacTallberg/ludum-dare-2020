using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Targetable
{

    public int matterRemaining = 100;
    UIManager resourceManager;
    public bool randomMatter;
    public float scaledSize;

    public float initialSize;
    public float initialScale;
    public int minMinerals = 25;
    public int maxMinerals = 200;

    // Start is called before the first frame update
    void Start()
    {
        if (randomMatter == true)
        {
            matterRemaining = Random.Range(minMinerals, maxMinerals);

        }
        initialScale = matterRemaining / 75 * transform.localScale.x;
        initialSize = (float)matterRemaining;
        resourceManager = GameObject.Find("Managers").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scaledSize = (float)matterRemaining / initialSize;
        Vector3 tempScale = transform.localScale;
        tempScale = new Vector3(initialScale * scaledSize,
                            initialScale * scaledSize,
                            initialScale * scaledSize);
        transform.localScale = tempScale;
    }

    public void mineMatter(int amountToMine)
    {
        int matterMined = Mathf.Min(amountToMine, matterRemaining);
        matterRemaining -= matterMined;
        resourceManager.totalMinerals += matterMined;
        if (matterRemaining <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
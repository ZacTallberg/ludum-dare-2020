using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public Color c1 = Color.white;
    public Color c2 = Color.cyan;

    private LineRenderer laserLine;

    // Start is called before the first frame update
    void Start()
    {
        laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.widthMultiplier = 0.2f;
        laserLine.positionCount = 2;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        laserLine.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator pulseLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(0.1f);
        laserLine.enabled = false;
    }

    public void shootAt(Vector3 target)
    {
        Vector3 startPos = this.gameObject.transform.position;
        Vector3 endPos = target;
        laserLine.SetPosition(0, startPos);
        laserLine.SetPosition(1, endPos);

        StartCoroutine(pulseLaser());
    }
}

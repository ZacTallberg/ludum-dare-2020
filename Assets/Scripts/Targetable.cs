using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetable : MonoBehaviour
{

    public int health;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Applies damage to the Targetable.
    /// </summary>
    /// <param name="damageDealt">The amount of damage to deal in this hit.</param>
    public virtual void takeDamage(int damageDealt)
    {
        health -= damageDealt;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

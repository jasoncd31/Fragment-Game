using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (tag.Equals("Health"))
            {
                playerStats.changeHealth(1);
            }
            else if (tag.Equals("Hazard"))
            {
                playerStats.changeHealth(-1);
            }
            Destroy(gameObject);
        }
    }
}

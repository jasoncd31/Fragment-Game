using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);

            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.changeHealth(-damage);
            }
        }

        else if (other.tag == "Impenetrable")
        {
            Debug.Log("Vandalism is not cool");
            Destroy(gameObject);
        }
    }
}

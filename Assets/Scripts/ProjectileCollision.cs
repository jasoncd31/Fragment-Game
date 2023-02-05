using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField]
    private int damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("You are most unkind");
            Destroy(gameObject);

            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        else if (other.tag == "Impenetrable")
        {
            Debug.Log("Vandalism is not cool");
            Destroy(gameObject);
        }
    }
}

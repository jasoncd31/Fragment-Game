using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
   public WeaponController wp;
   public int damage;

   private void OnTriggerEnter(Collider other)
   {
       if (other.tag == "Enemy" && wp.isAttacking)
       {
           Debug.Log("oof");
           EnemyController enemy = other.GetComponent<EnemyController>();
           if (enemy != null)
           {
               enemy.TakeDamage(damage);
           }
       }
   }
}

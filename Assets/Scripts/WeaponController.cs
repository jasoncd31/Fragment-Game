using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Weapon;
    public bool isAttacking = false;
    private bool canAttack = true;
    private float attackCooldown = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            // Debug.Log("hey mama hey mama look around");
            if (canAttack) 
            {
                Strike();
            }
        }
    }

    public void Strike()
    {
        isAttacking = true;
        canAttack = false;
        Animator anim = Weapon.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        Invoke("Reset", attackCooldown);
    }

    public void Reset()
    {
        isAttacking = false;
        canAttack = true;
    }

    IEnumerator StopAttack()
    {
        isAttacking = false;
        //Debug.Log("I'm done attacking");
        yield return new WaitForSeconds(1f);
    }
}

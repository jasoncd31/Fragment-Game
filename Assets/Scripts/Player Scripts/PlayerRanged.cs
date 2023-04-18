using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRanged : MonoBehaviour    
{
    public GameObject proj;
    public GameObject player;

    private Vector3 fireballPos;
    private Vector3 offset;
    public float despawnTime = .5f;

    //modify this value to increase or decrease projectile
    [SerializeField]
    private float projVelocity = 50f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fireballPos = player.transform.position;
            //spawn fireball
            GameObject fireball = Instantiate(proj, fireballPos, Quaternion.identity);
            //get rigidbody component
            Rigidbody fireballRigid = fireball.GetComponent<Rigidbody>();
            //add velocity to rigidbody component
            fireballRigid.velocity = transform.TransformDirection(Vector3.forward * projVelocity);
            Destroy(fireball, despawnTime);

            
        }
    }
    
}

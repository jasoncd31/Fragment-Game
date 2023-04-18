using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DartFlowerController : EnemyController
{
    public GameObject player;
    private Animator flowerAnimator;
    private bool dead = false;
    public float aggroDistance = 200f;
    private float dropSpeed;
    Vector3 toPlayer;

    public DartFlowerController()
    {
        health = 3;
        attackDamage = -1;
        enemyDrop = null;
        bullet = null;
        projVelocity = 0f;
        lifespan = 0f;
        state = State.Idle;
    }
    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        flowerAnimator = GetComponent<Animator>();
        dropSpeed = 5f;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        toPlayer = player.transform.position - transform.position;
        // Debug.Log(player.transform.position);
        switch(state)
        {
            case State.Idle:
                if (toPlayer.magnitude < aggroDistance && state != State.Striking)
                {
                    state = State.Striking;
                }
                break;
            case State.Alerted:
                break;
            case State.Striking:
                flowerAnimator.SetTrigger("Attacking");
                break;
            case State.Returning:
                break;
            case State.Dead:
            if (!dead) 
            {
                flowerAnimator.SetTrigger("Dead");
                dead = true;
            }
                break;
        }
        Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        bulletRigid.AddForce(new Vector3(0, -2000, 0), ForceMode.Impulse);
    }

    private void DartDrop()
    {
        Instantiate(bullet, new Vector3(player.transform.position.x, player.transform.position.y + 200, player.transform.position.z), new Quaternion(0, 0, -180, 1));
    }

    private void toIdle()
    {
        state = State.Idle;
    }
}

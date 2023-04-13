using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DartFlowerController : EnemyController
{
    public GameObject player;
    private Animator flowerAnimator;
    private bool dead = false;
    public float aggroDistance = 20f;
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
    }

    // Update is called once per frame
    private void Update()
    {
        toPlayer = player.transform.position - transform.position;
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
    }

    private void toIdle()
    {
        state = State.Idle;
    }
}

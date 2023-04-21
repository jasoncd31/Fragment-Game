using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PufferFrogController : EnemyController
{
    public GameObject player;
    public float aggroDistance = 50f;
    public float retreatDistance = 100f;
    public float attackRange = 10f;
    Vector3 toPlayer;
    Vector3 toAnchor;

    private Animator frogAnimator;

    public PufferFrogController()
    {
        health = 1;
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
        anchor = transform.position;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        frogAnimator = GetComponent<Animator>();
        agent.isStopped = true;
    }

    // Update is called once per frame
    private void Update()
    {
        toPlayer = player.transform.position - transform.position;
        toAnchor = anchor - transform.position;
        switch(state)
        {
            case State.Idle:
                if (toPlayer.magnitude < aggroDistance)
                {
                    state = State.Alerted;
                    frogAnimator.SetBool("Alert", true);
                }
                break;
            case State.Alerted:
                agent.destination = player.transform.position;
                if (toAnchor.magnitude > retreatDistance) 
                {
                    state = State.Returning;
                }
                if (toPlayer.magnitude < attackRange)
                {
                    state = State.Striking;
                }
                break;
            case State.Returning:
                agent.destination = anchor;
                if (toAnchor.magnitude < 5f)
                {
                    state = State.Idle;
                }
                break;
            case State.Striking:
                StopMoving();
                frogAnimator.SetTrigger("Attacking");
                break;
            case State.Dead:
                StopExisting();
                break;
        }
    }

    private void toIdle()
    {
        state = State.Idle;
        agent.isStopped = false;
        frogAnimator.SetBool("Alert", false);
    }

    private void StopMoving()
    {
        agent.isStopped = true;
    }

    private void ResumeMoving()
    {
        agent.isStopped = false;
    }
}

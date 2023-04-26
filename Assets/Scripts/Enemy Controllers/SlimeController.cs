using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeController : EnemyController
{
    public GameObject player;
    public float aggroDistance = 20f;
    public float retreatDistance = 50f;
    Vector3 toPlayer;
    Vector3 toAnchor;

    public SlimeController()
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
                }
                break;
            case State.Alerted:
                agent.destination = player.transform.position;
                if (toAnchor.magnitude > retreatDistance) 
                {
                    state = State.Returning;
                }
                break;
            case State.Returning:
                agent.destination = anchor;
                if (toAnchor.magnitude < 5f)
                {
                    state = State.Idle;
                }
                break;
            case State.Dead:
                StopExisting();
                break;
        }
    }
}

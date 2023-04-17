using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBossController : EnemyController
{
    [SerializeField]
    private GameObject player;
    private Vector3 toPlayer;
    private Animator bossAnimator;

    [SerializeField]
    private float aggroDistance;
    private bool dead = false;
    private bool charging = false;
    private bool slamming = false;

    public GolemBossController()
    {
        health = 30;
        attackDamage = -3;
        enemyDrop = null;
        bullet = null;
        projVelocity = 0f;
        lifespan = 0f;
        state = State.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        bossAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        toPlayer = player.transform.position - transform.position;

        switch(state)
        {
            case State.Idle:
                if (toPlayer.magnitude < aggroDistance)
                {
                    state = State.Charging;
                }
                break;
            case State.Charging:
                if (!charging)
                {
                    bossAnimator.SetBool("Charging", true);
                    charging = true;
                    // Check for a collision, if a with wall then stun, otherwise slam.
                }
                break;
            case State.Slam:
                if (!slamming)
                {
                    // If is only here to stop animation from resetting. After this, should return to charging
                }
                break;
            case State.Stunned:
                break;
            case State.Dead:
            if (!dead) 
            {
                StopExisting();
            }
                break;
        }
    }
}

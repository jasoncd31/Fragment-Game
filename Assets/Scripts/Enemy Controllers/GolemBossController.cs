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
    private bool aggroed = false;
    private bool charging = false;
    private bool slamming = false;
    private bool stunned = false;

    public GolemBossController()
    {
        health = 30;
        attackDamage = 0;
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
        if (transform.position.y < -10)
        {
            state = State.Dead;
        }
        toPlayer = player.transform.position - transform.position;

        switch(state)
        {
            case State.Idle:
                if (toPlayer.magnitude < aggroDistance)
                {
                    state = State.Alerted;
                }
                break;
            case State.Alerted:
                if (!aggroed)
                {
                    bossAnimator.SetTrigger("Alert");
                }
                break;
            case State.Charging:
                stunned = false;
                slamming = false;
                transform.Translate(0, 0, 1.0f);
                if (!charging)
                {
                    LookToPlayer();
                    bossAnimator.SetBool("Charging", true);
                    charging = true;
                    // Check for a collision, if a with wall then stun, otherwise slam.
                }
                break;
            case State.Slam:
                if (!slamming)
                {
                    charging = false;
                    stunned = false;
                    // If is only here to stop animation from resetting. After this, should return to charging
                    bossAnimator.SetTrigger("Slam");
                    slamming = true;
                }
                break;
            case State.Stunned:
                if (!stunned)
                {
                    slamming = false;
                    charging = false;
                    bossAnimator.SetTrigger("Bonk");
                    transform.Translate(0, 0, -50.0f);
                    stunned = true;
                }
                break;
            case State.Dead:
            if (!dead) 
            {
                StopExisting();
            }
                break;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (charging){
            attackDamage = -3;
            if (other.gameObject.tag == "Impenetrable")
            {
                attackDamage = 0;
                state = State.Stunned;
            }
            else if (other.gameObject.tag == "Player")
            {
                attackDamage = 0;
                state = State.Slam;
            }
        }
    }

    private void toCharging()
    {
        state = State.Charging;
    }

    private void toAlert()
    {
        state = State.Alerted;
    }

    private void toStunned()
    {
        state = State.Stunned;
    }

    private void toSlam()
    {
        state = State.Slam;
    }

    private void LookToPlayer()
    {
        transform.LookAt(player.transform);
    }
}

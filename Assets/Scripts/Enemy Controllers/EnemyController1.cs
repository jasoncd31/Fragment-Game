using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public int health = 3;
    [SerializeField]
    public int attackDamage = -1;
    public GameObject enemyDrop;

    [SerializeField]
    public GameObject bullet;
    public float projVelocity = 20f;
    public float lifespan = 3f;
    public Vector3 anchor;
    //private Random rng = new Random();

    public Transform goal;

    public NavMeshAgent agent;
    public enum State
    {
        Idle,
        Alerted,
        Striking,
        Returning,
        Dead,
        Charging,
        Slam,
        Stunned
    }
    public State state;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        state = State.Idle;
        anchor = transform.position;
    }

    private void Update()
    {
        agent.destination = goal.position;
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("pewpew");
            Attack();
        }
    }

    private void Attack() 
    {
        GameObject dangerball = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody dangerballRigid = dangerball.GetComponent<Rigidbody>();
        dangerballRigid.velocity = transform.TransformDirection(Vector3.forward * projVelocity);
        Destroy(dangerball, lifespan);
    }

    // Currently unused, unsure if enemies should deal "contact damage".
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();

            player.changeHealth(attackDamage);
        }
    }

    /* 
        Enemy should take damage according to power of source,
        and if health is 0 or less, enemy should be killed to death.
    */
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            state = State.Dead;
        }

    }

    /*
        The "killed to death part" of the above.
    */
    public void StopExisting()
    {
        int numDrops = Random.Range(3, 6);
        DropCollectibles(numDrops);
        Destroy(gameObject);
    }

    /*
        Helper function to spawn drop objects, which will
        spawn experience or other drops when hitting the ground.
    */
    private void DropCollectibles(int numDrops)
    {
        for (int drop = 0; drop < numDrops; drop++)
        {
            SpawnCollectible();
        }
    }

    private void SpawnCollectible()
    {
        Vector3 CollectiblePosition = transform.position;
        GameObject CollectibleSpawner = Instantiate(enemyDrop, CollectiblePosition, Quaternion.Euler(0,0,270));
        Rigidbody CollectibleBody = CollectibleSpawner.GetComponent<Rigidbody>();
        float randX = Random.Range(-1, 1);
        float randZ = Random.Range(-1, 1);
        float randY = Random.Range(0.7f, 0.8f);
        Vector3 CollectibleLaunch = new Vector3(randX, randY, randZ);

        CollectibleBody.velocity = transform.TransformDirection(CollectibleLaunch * (Random.Range(0.7f, 1.0f) * 4));
    }
}

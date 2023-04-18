using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int health = 3;
    [SerializeField]
    private int attackDamage = -1;
    public GameObject enemyDrop;
    //private Random rng = new Random();

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();

            player.changeHealth(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            StopExisting();
        }

    }

    private void StopExisting()
    {
        int numDrops = Random.Range(3, 6);
        DropCollectibles(numDrops);
        Destroy(gameObject);
    }

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
        GameObject CollectibleSpawner = Instantiate(enemyDrop, CollectiblePosition, Quaternion.identity);
        Rigidbody CollectibleBody = CollectibleSpawner.GetComponent<Rigidbody>();
        float randX = Random.Range(-1, 1);
        float randZ = Random.Range(-1, 1);
        float randY = Random.Range(0.7f, 0.8f);
        Vector3 CollectibleLaunch = new Vector3(randX, randY, randZ);

        CollectibleBody.velocity = transform.TransformDirection(CollectibleLaunch * (Random.Range(0.7f, 1.0f) * 4));
    }
}

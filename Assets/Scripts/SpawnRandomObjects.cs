using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObjects : MonoBehaviour
{
    public List<GameObject> plants = new List<GameObject>();
    public GameObject parent;
    public int numOfObj = 40;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < numOfObj; i++)
        {
            //Vector3 rotation = new Vector3(0.0f, Random.Range(0.0f, 180.0f), 0, 0f);
            Instantiate(plants[0], new Vector3(Random.Range(-50.0f,50.0f) + parent.transform.position.x, 0.0f, Random.Range(-50.0f, 50.0f) + parent.transform.position.z), Quaternion.identity);
        }

        for (var i = 0; i < 2; i++)
        {
            Instantiate(plants[1], new Vector3(Random.Range(-40.0f, 40.0f) + parent.transform.position.x, 0.0f, Random.Range(-40.0f, 40.0f) + parent.transform.position.z), Quaternion.identity);
        }

        for (var i = 0; i < 1; i++)
        {
            Instantiate(plants[2], new Vector3(Random.Range(-40.0f, 40.0f) + parent.transform.position.x, 0.0f, Random.Range(-40.0f, 40.0f) + parent.transform.position.z), Quaternion.identity);
        }

        for (var i = 0; i < 1; i++)
        {
            Instantiate(plants[3], new Vector3(Random.Range(-40.0f, 40.0f) + parent.transform.position.x, 0.0f, Random.Range(-40.0f, 40.0f) + parent.transform.position.z), Quaternion.identity);
        }
    }
}

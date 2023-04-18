using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObjects : MonoBehaviour
{
    public List<GameObject> plants = new List<GameObject>();
    public GameObject parent;
    public int numOfObj = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < numOfObj; i++)
        {
            //Vector3 rotation = new Vector3(0.0f, Random.Range(0.0f, 180.0f), 0, 0f);
            Instantiate(plants[Random.Range(0,3)], new Vector3(Random.Range(-10.0f,10.0f) + parent.transform.position.x, 0.0f, Random.Range(-10.0f, 10.0f) + parent.transform.position.z), Quaternion.identity);
            Debug.LogWarning("Spawned object");
        }
    }
}

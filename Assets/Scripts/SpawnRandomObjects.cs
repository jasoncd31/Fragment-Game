using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObjects : MonoBehaviour
{
    public GameObject objectRef;
    public int numOfObj = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < numOfObj; i++)
        {
            //Vector3 rotation = new Vector3(0.0f, Random.Range(0.0f, 180.0f), 0, 0f);
            Instantiate(objectRef, new Vector3(Random.Range(-50.0f,50.0f), 0.0f, Random.Range(-50.0f, 50.0f)), Quaternion.identity);
        }
    }
}

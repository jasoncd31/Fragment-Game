using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomPillars : MonoBehaviour
{
    public GameObject pillar;
    public int numOfPillars = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < numOfPillars; i++)
        {
            Instantiate(pillar, new Vector3(Random.Range(-50.0f,50.0f), Random.Range(0.0f, 2.5f), Random.Range(-50.0f, 50.0f)), Quaternion.identity);
        }
    }
}

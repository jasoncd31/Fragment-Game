using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explotion2 : MonoBehaviour
{
    public float delay = 7f;
    public float radius = 5f;
    public float force = 7000f;

    float countdown;
    bool hasExploded = false;

    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }
    void Explode()
    {
        Debug.Log("BOOM!");

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObjetc in colliders)
        {
          Rigidbody rb = nearbyObjetc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
                Debug.Log("Force!");
                
            }
            Destruction dest = nearbyObjetc.GetComponent<Destruction>();
            if (dest != null)
            {
                dest.Destroy();
            }

        }

        Destroy(gameObject);
        hasExploded = false;
    }
}

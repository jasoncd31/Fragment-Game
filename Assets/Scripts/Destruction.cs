//Erik
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    public GameObject destroyedVersion;

    public void Destroy()
    {
        // Call DestroyGameObject() method after a delay of 1 second
        Invoke("DestroyGameObject", 0.5f);
    }

    void DestroyGameObject()
    {
        // Destroy the game object
        Destroy(gameObject);
    }
}


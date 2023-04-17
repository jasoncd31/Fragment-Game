using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollow : MonoBehaviour
{
    public Transform playerObject;
    public Vector3 offset;
    // Update is called once per frame
    void Update()
    {
        transform.position = playerObject.position + offset;
        transform.RotateAround(transform.position, Vector3.down,transform.position.y);
    }
}

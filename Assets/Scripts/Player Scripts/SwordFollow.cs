using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollow : MonoBehaviour
{
    public Transform playerObject;
    public Vector3 offset;

    private bool swinging = false;
    private Animator swordCon;

    private void Start()
    {
        swordCon = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = playerObject.position + offset;
        transform.RotateAround(transform.position, Vector3.down,transform.position.y);
        if (Input.GetMouseButtonDown(0))
        {
            if (!swinging)
            {
                swordCon.SetTrigger("Attack");
                swinging = true;
            }
        }
    }
    private void SwingDone()
    {
        swinging = false;
    }
}

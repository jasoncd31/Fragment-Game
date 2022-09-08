using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 currPlayer;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        currPlayer = player.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        currPlayer = player.transform.position;
         
        if (Input.GetAxis("Jump") != 0)
        {
            transform.RotateAround(currPlayer, -Vector3.up, 20 * Time.deltaTime);
        }
    }
    // LateUpdate is called after all other update functions
    private void LateUpdate()
    {
        //doesn't account for rotation, probably
        transform.position = player.transform.position + offset;
    }
}

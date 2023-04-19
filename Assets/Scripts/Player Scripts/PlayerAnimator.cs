// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerAnimator : MonoBehaviour
// {
//     private Animator playerAnimator;

//     private Vector2 move2D;

//     [SerializeField]
//     private PlayerController player;
//     // Start is called before the first frame update
//     void Start()
//     {
//                 playerAnimator = gameObject.GetComponent<Animator>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
//         {
//             PlayMovingAnimation();
//         }
//         else
//         {
//             playerAnimator.SetBool("Forward", false);
//             playerAnimator.SetBool("Backward", false);
//         }

        


//     }

//         private void PlayMovingAnimation()
//     {
//         move2D = new(Input.GetAxis("Horizontal"),  Input.GetAxis("Vertical"));
//         Vector2 lookPos2D = new(player.lookPosPublic.x,player.lookPosPublic.z);

        
//         //Debug.Log("move: "+ move2D + "LookPos: " + lookPos2D);
//         //Debug.Log("Rotate Angle: " + Vector2.Angle(move2D, lookPos2D));
//         if (Vector2.Angle(move2D, lookPos2D) < 90.0f)
//         {
//             playerAnimator.SetBool("Forward", true);
//             playerAnimator.SetBool("Backward", false);
//         }
//         if (Vector2.Angle(move2D, lookPos2D) > 90.0f)
//         {
//             playerAnimator.SetBool("Forward", false);
//             playerAnimator.SetBool("Backward", true);
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    private PlayerStats pStats;
    private Animator playerAnimator;
    private Vector3 move;
    private Vector3 lookPos;

    [SerializeField]
    public Camera mainCam;
    private Vector3 playerVelocity;

    [SerializeField]
    private float playerSpeed = 150.0f;

    private float basePlayerSpeed;
    private float sprintPlayerSpeedMultiplier = 1.25f;
    private float gravity = -2.0f;

    private bool isDashing = false; //is the player currently dashing
    private float dashTime = 0f; // how long the increased playerSpeed/dashTime goes for
    private float dashPlayerSpeedMultiplier = 1.5f;
    float dashSpeed; // how fast the player goes during the dash
    float dashCooldown = 3f; //how long until the player can dash again
    float canDash = 0f; //Time.time + dashCooldown = canDash. Used in the comparison

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        pStats = controller.gameObject.GetComponent<PlayerStats>();
        playerAnimator = gameObject.GetComponent<Animator>();
        basePlayerSpeed = playerSpeed;
        dashSpeed = basePlayerSpeed * dashPlayerSpeedMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        Sprinting();
        RotatePlayer();

        if (!isDashing)
        {
            StopCoroutine(DashCo());
            move = new Vector3(Input.GetAxis("Horizontal"), gravity, Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);
            // Debug.Log(move * Time.deltaTime * playerSpeed);
            // Debug.Log(playerSpeed);
            if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
            {
                PlayMovingAnimation();
            }
            else
            {
                playerAnimator.SetBool("Forward", false);
                playerAnimator.SetBool("Backward", false);
            }
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > canDash)
            {
                dashTime = 0f;
                StartCoroutine(DashCo());
            }
        }

        //die if you fall off floor
        if (transform.position.y < -30)
        {
            pStats.deathReset();
        }

    }

    private IEnumerator DashCo()
    {
        isDashing = true;
        Vector3 moveDash = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        while (dashTime < 0.25f)
        {
            controller.Move(moveDash * Time.deltaTime * dashSpeed);
            dashTime += Time.deltaTime;
            // playerAnimator.SetTrigger("Dash");
            yield return null;
        }
        isDashing = false;
        canDash = Time.time + dashCooldown;
        yield return null;
    }

    void Sprinting()
    {
        if (Input.GetAxis("Sprint") > 0)
        {
            playerSpeed *= sprintPlayerSpeedMultiplier;
        }
        else
        {
            playerSpeed = basePlayerSpeed;
        }
    }
    private void RotatePlayer()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            lookPos = hit.point - transform.position;
            lookPos.y = transform.position.y;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
            // ducttape tapes it to stay upright
            Vector3 ductTape = new Vector3(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(ductTape);
        }
    }

    private void PlayMovingAnimation()
    {
        Vector2 lookPos2D = new(lookPos.x,lookPos.z);
        Vector2 move2D = new(move.x, move.z);
        
        //Debug.Log("move: "+ move2D + "LookPos: " + lookPos2D);
        //Debug.Log("Rotate Angle: " + Vector2.Angle(move2D, lookPos2D));
        if (Vector2.Angle(move2D, lookPos2D) < 90.0f)
        {
            playerAnimator.SetBool("Forward", true);
            playerAnimator.SetBool("Backward", false);
        }
        if (Vector2.Angle(move2D, lookPos2D) > 90.0f)
        {
            playerAnimator.SetBool("Forward", false);
            playerAnimator.SetBool("Backward", true);
        }
    }
}

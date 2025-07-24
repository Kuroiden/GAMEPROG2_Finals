using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Linked Scripts")]

    [Header("Game Objects")]
    CharacterController PlayerCtrl;
    public Camera PlayerCam;
    public Slider Health;
    public Slider Stamina;

    [Header("Player Movement")]
    public bool canMove;
    Vector3 updatePos = Vector3.zero;
    public float playerSpd;
    public float jumpForce;
    public float gravity;

    [Header("Camera Settings")]
    float rotationX = 0.0f;
    float rotationXLimit = 45.0f;
    float camSensitivity = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCtrl = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerForward = transform.TransformDirection(Vector3.forward);
        Vector3 playerRight = transform.TransformDirection(Vector3.right);
        float playerVeloX = canMove ? playerSpd * Input.GetAxis("Vertical") : 0.0f;
        float playerVeloZ = canMove ? playerSpd * Input.GetAxis("Horizontal") : 0.0f;

        if (!PlayerCtrl.isGrounded) updatePos.y -= gravity * Time.deltaTime;

        float updateYPos = updatePos.y;

        updatePos = (playerVeloX * playerForward) + (playerVeloZ * playerRight);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * camSensitivity;
            rotationX = Mathf.Clamp(rotationX, -rotationXLimit, rotationXLimit);

            if (Input.GetButton("Jump") && PlayerCtrl.isGrounded)
            {
                updatePos.y = jumpForce;
            }
            else
            {
                updatePos.y = updateYPos;
            }

            PlayerCam.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
            transform.rotation *= Quaternion.Euler(0.0f, (Input.GetAxis("Mouse X") * camSensitivity), 0.0f);

            PlayerCtrl.Move(updatePos * Time.deltaTime);
        }
    }
}

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
    public Slider Stamina;
    public GameObject[] HealthOverlay;
    public GameObject Win;
    public GameObject Lose;

    [Header("Player Movement")]
    public bool canMove;
    Vector3 updatePos = Vector3.zero;
    public float playerSpd;
    public float jumpForce;
    public float gravity;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchSpeed = 2.0f;
    public float crouchCamHeightOffset = -0.5f;
    private bool isCrouching = false;
    private float defaultSpeed;
    private Vector3 standCamPos;
    private Vector3 crouchCamPos;

    [Header("Camera Settings")]
    float rotationX = 0.0f;
    float rotationXLimit = 45.0f;
    float camSensitivity = 2.5f;

    [Header("Lean Settings")]
    public float leanAngle = 15f;
    public float leanOffset = 0.2f;
    public float leanSpeed = 5f;

    float currentLean = 0f;
    float targetLean = 0f;
    float currentOffset = 0f;
    float targetOffset = 0f;

    Vector3 defaultCamPos;

    [Header("Player Stats")]
    public int HP = 5;
    public int MaxStamina;
    public int CurrStamina;
    public int Ammo;

    [Header("Player Win/Lose")]
    public bool p_Win = false;
    public bool p_Lose = false;

    void Start()
    {
        PlayerCtrl = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerCtrl.height = standHeight;
        defaultCamPos = PlayerCam.transform.localPosition;
        standCamPos = defaultCamPos;
        crouchCamPos = defaultCamPos + new Vector3(0f, crouchCamHeightOffset, 0f);

        defaultSpeed = playerSpd;
    }

    void Update()
    {
        Vector3 playerForward = transform.TransformDirection(Vector3.forward);
        Vector3 playerRight = transform.TransformDirection(Vector3.right);
        float playerVeloX = canMove ? playerSpd * Input.GetAxis("Vertical") : 0.0f;
        float playerVeloZ = canMove ? playerSpd * Input.GetAxis("Horizontal") : 0.0f;

        if (!PlayerCtrl.isGrounded)
            updatePos.y -= gravity * Time.deltaTime;

        float updateYPos = updatePos.y;

        updatePos = (playerVeloX * playerForward) + (playerVeloZ * playerRight);

        switch (HP)
        {
            case 0:
                p_Lose = true;

                HealthOverlay[0].SetActive(true);
                HealthOverlay[1].SetActive(false);
                HealthOverlay[2].SetActive(false);
                HealthOverlay[3].SetActive(false);
                HealthOverlay[4].SetActive(true);
                break;

            case 1:
                HealthOverlay[0].SetActive(false);
                HealthOverlay[1].SetActive(false);
                HealthOverlay[2].SetActive(false);
                HealthOverlay[3].SetActive(true);
                HealthOverlay[4].SetActive(false);
                break;

            case 2:
                HealthOverlay[0].SetActive(false);
                HealthOverlay[1].SetActive(false);
                HealthOverlay[2].SetActive(true);
                HealthOverlay[3].SetActive(false);
                HealthOverlay[4].SetActive(false);
                break;

            case 3:
                HealthOverlay[0].SetActive(false);
                HealthOverlay[1].SetActive(true);
                HealthOverlay[2].SetActive(false);
                HealthOverlay[3].SetActive(false);
                HealthOverlay[4].SetActive(false);
                break;

            case 4:
                HealthOverlay[0].SetActive(true);
                HealthOverlay[1].SetActive(false);
                HealthOverlay[2].SetActive(false);
                HealthOverlay[3].SetActive(false);
                HealthOverlay[4].SetActive(false);
                break;

            case 5:
                HealthOverlay[0].SetActive(false);
                HealthOverlay[1].SetActive(false);
                HealthOverlay[2].SetActive(false);
                HealthOverlay[3].SetActive(false);
                HealthOverlay[4].SetActive(false);
                break;
        }

        if (p_Win || p_Lose)
        {
            canMove = false;

            if (p_Win) Win.SetActive(true);
            else if (p_Lose) Lose.SetActive(true);

            if (Input.GetKey(KeyCode.R))
            {
                p_Lose = false;
                p_Win = false;
            }
        }
        else
        {
            canMove = true;

            Win.SetActive(false);
            Lose.SetActive(false);
        }

        if (canMove)
        {
            // Mouse Look Up/Down
            rotationX += -Input.GetAxis("Mouse Y") * camSensitivity;
            rotationX = Mathf.Clamp(rotationX, -rotationXLimit, rotationXLimit);

            // Toggle Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouching = !isCrouching;

                if (isCrouching)
                {
                    PlayerCtrl.height = crouchHeight;
                    playerSpd = crouchSpeed;
                }
                else
                {
                    PlayerCtrl.height = standHeight;
                    playerSpd = defaultSpeed;
                }
            }

            // Jump
            if (Input.GetButton("Jump") && PlayerCtrl.isGrounded)
                updatePos.y = jumpForce;
            else
                updatePos.y = updateYPos;

            // Lean Input
            if (Input.GetKey(KeyCode.Q))
            {
                targetLean = leanAngle;
                targetOffset = -leanOffset;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                targetLean = -leanAngle;
                targetOffset = leanOffset;
            }
            else
            {
                targetLean = 0f;
                targetOffset = 0f;
            }

            // Smooth Lean and Offset
            currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
            currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * leanSpeed);

            // Determine camera target position based on crouch
            Vector3 camTargetPos = isCrouching ? crouchCamPos : standCamPos;
            camTargetPos += new Vector3(currentOffset, 0f, 0f);

            // Smooth camera move and lean
            PlayerCam.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, currentLean);
            PlayerCam.transform.localPosition = Vector3.Lerp(PlayerCam.transform.localPosition, camTargetPos, Time.deltaTime * leanSpeed);

            // Rotate Player Left/Right
            transform.rotation *= Quaternion.Euler(0.0f, Input.GetAxis("Mouse X") * camSensitivity, 0.0f);

            // Move
            PlayerCtrl.Move(updatePos * Time.deltaTime);
        }
    }
}

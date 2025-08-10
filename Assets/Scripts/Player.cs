using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Linked Scripts")]

    [Header("Game Objects")]
    CharacterController PlayerCtrl;
    public Text Ammo_Txt;
    public Text HP_Txt;
    public Camera PlayerCam;
    public Slider Stamina;
    public GameObject[] HealthOverlay = new GameObject[5];
    public GameObject[] movementIcon = new GameObject[3];
    public GameObject[] weaponIcon = new GameObject[2];
    public GameObject pistol;
    public GameObject knife;

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

    [Header("Weapon Settings")]
    public float attackRange = 2f;
    public int damage = 50;
    public float attackCooldown = 0.8f;
    public LayerMask enemyLayers;

    public Transform attackPoint; // Where the knife "hits" from
    private float nextAttackTime = 0f;

    bool toggleKnife;
    
    public Rigidbody bullet;
    public float bulletSpeed;

    [Header("Player Stats")]
    public int HP = 5;
    public int MaxStamina;
    public float CurrStamina;
    public int Ammo;

    [Header("Player Win/Lose")]
    public bool p_Win = false;
    public bool p_Lose = false;

    void Awake()
    {
        toggleKnife = false;

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
        Ammo_Txt.text = Ammo.ToString();
        HP_Txt.text = HP.ToString();

        Vector3 playerForward = transform.TransformDirection(Vector3.forward);
        Vector3 playerRight = transform.TransformDirection(Vector3.right);

        // Move character
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        if (isRunning && CurrStamina > 0)
        {
            CurrStamina -= Time.deltaTime;
        }
        else if (!isRunning && CurrStamina < MaxStamina) CurrStamina += Time.deltaTime * 1.5f;

        if (isRunning & CurrStamina <= 0) isRunning = false;

        Stamina.value = CurrStamina;
        float playerVeloX = canMove ? (isRunning ? playerSpd * 1.5f : playerSpd) * Input.GetAxis("Vertical") : 0.0f;
        float playerVeloZ = canMove ? (isRunning ? playerSpd * 1.5f : playerSpd) * Input.GetAxis("Horizontal") : 0.0f;

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

        if (p_Win || p_Lose) canMove = false;
        else canMove = true;

        if (canMove)
        {
            // Updates movement state icon
            int moveState = 0;

            if (isRunning) moveState = 1;
            else if (isCrouching) moveState = 2;
            else moveState = 0;

            switch (moveState)
            {
                case 0:
                    movementIcon[0].SetActive(true);
                    movementIcon[1].SetActive(false);
                    movementIcon[2].SetActive(false);
                    break;

                case 1:
                    movementIcon[0].SetActive(false);
                    movementIcon[1].SetActive(true);
                    movementIcon[2].SetActive(false);
                    break;

                case 2:
                    movementIcon[0].SetActive(false);
                    movementIcon[1].SetActive(false);
                    movementIcon[2].SetActive(true);
                    break;
            }

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

            if (Input.GetKeyDown(KeyCode.X)) toggleKnife = !toggleKnife;
            
            playerAttack();

            // Move
            PlayerCtrl.Move(updatePos * Time.deltaTime);
        }
    }

    void playerAttack()
    {
        if (toggleKnife) {
            weaponIcon[0].SetActive(false);
            weaponIcon[1].SetActive(true);

            pistol.SetActive(false);
            knife.SetActive(true);

            // Knife attack
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    KnifeAttack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            weaponIcon[0].SetActive(true);
            weaponIcon[1].SetActive(false);

            pistol.SetActive(true);
            knife.SetActive(false);

            if (Ammo > 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ammo--;
                    Rigidbody instantiatedProjectile = Instantiate(bullet, attackPoint.transform.position, attackPoint.transform.rotation);
                    instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, bulletSpeed));
                }
            }
        }
    }

    void KnifeAttack()
    {
        // Detect enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            // Apply damage
            Enemy health = enemy.GetComponent<Enemy>();
            if (health != null)
                health.e_HP -= 1;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

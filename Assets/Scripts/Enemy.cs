using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("Linked Scripts")]
    public GameManager gameManager;
    public AIManager aiManager;
    public EnemyFOV fov;

    [Header("Game Objects")]
    GameObject player;
    //public GameObject weapon;

    [Header("Components")]
    public NavMeshAgent enemy_Move;
    public Transform[] enemyPOI;

    [Header("Parameters")]
    public int enemyDestination;
    public float distance;

    public bool isHostile;

    public float interestTimer;

    public int e_HP = 5;

    void Start()
    {
        fov = this.GetComponent<EnemyFOV>();

        enemy_Move = GetComponent<NavMeshAgent>();
        enemy_Move.autoBraking = false;

        player = GameObject.Find("Player");

        enemyDestination = 0;
        distance = 1f;
        isHostile = false;

        if (aiManager.isAIAwake)
        {
            MoveDestination();
        }
    }

    void Update()
    {
        if (e_HP > 0)
        {
            if (aiManager.isAIAwake)
            {
                if (!enemy_Move.pathPending && enemy_Move.remainingDistance < distance)
                {
                    gameManager.playerFound = false;

                    if (fov.foundPlayer.Count == 0)
                    {
                        if (interestTimer > 0)
                        {
                            interestTimer -= Time.deltaTime;
                            enemy_Move.ResetPath();
                        }
                        else if (interestTimer <= 0)
                        {
                            MoveDestination();
                            interestTimer = 0;
                        }
                    }

                }
                else if (fov.foundPlayer.Count != 0)
                {
                    gameManager.playerFound = true;

                    enemy_Move.SetDestination(player.transform.position);

                    interestTimer = 5f;
                }
            }
            else
            {
                gameManager.playerFound = false;

                enemy_Move.autoBraking = true;
            }
        }
        else
        {
            enemy_Move.ResetPath();
            float enemyFall = this.gameObject.transform.position.y - 1f;

            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, enemyFall, this.gameObject.transform.position.z);

            if (this.gameObject.transform.position.y < -3f) Destroy(this);
        }
    }
    void MoveDestination()
    { 
        if (enemyPOI.Length == 0)
        {
            enabled = false;
            return;
        }

        enemy_Move.destination = enemyPOI[enemyDestination].position;
        enemyDestination = (enemyDestination + 1) % enemyPOI.Length;
    }
    
}

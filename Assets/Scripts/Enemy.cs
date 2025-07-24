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
        if (aiManager.isAIAwake)
        {
            if (!enemy_Move.pathPending && enemy_Move.remainingDistance < distance && !isHostile)
            {
                gameManager.playerFound = false;

                if (interestTimer > 0) interestTimer -= Time.deltaTime;
                else if (interestTimer <= 0)
                {
                    MoveDestination();

                    interestTimer = 0;
                }
            }
            else if (isHostile)
            {
                gameManager.playerFound = true;

                enemy_Move.SetDestination(player.transform.position);
            }
        }
        else
        {
            gameManager.playerFound = false;

            enemy_Move.autoBraking = true;
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

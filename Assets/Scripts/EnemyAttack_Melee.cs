using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack_Melee : MonoBehaviour
{
    public EnemyFOV fov;
    public GameObject player_Obj;

    public bool hasShield;

    void Start()
    {
        fov = this.GetComponent<EnemyFOV>();
        player_Obj = GameObject.Find("Player");
    }

    void Update()
    {
        //Vector3 distanceFromPlayer = Vector3Distance(transform.position, player_Obj.transform.position);
        //Vector3 maxDistance = new Vector3(3f, transform.position.y, )

        if (fov.foundPlayer.Count != 0 )
        {
            attack();
        }
    }

    void attack()
    {
        if (hasShield)
        {

        }

        
    }
}

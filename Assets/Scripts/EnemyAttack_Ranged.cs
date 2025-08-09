using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack_Ranged : MonoBehaviour
{
    public EnemyFOV fov;

    public GameObject player_Obj;
    public Rigidbody bullet;

    public Transform barrel;

    public LineRenderer shotPath;

    public float bulletSpd = 2f;

    //public float atkRange;

    void Start()
    {
        fov = this.GetComponent<EnemyFOV>();
        player_Obj = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (fov.foundPlayer.Count != 0)
        {
            float distanceFromPlayer = Vector3.Distance(this.transform.position, player_Obj.transform.position);

            Vector3[] points = new Vector3[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, distanceFromPlayer) };
            shotPath.positionCount = 2;
            shotPath.SetPositions(points);
        }
        else
        {
            shotPath.positionCount = 0;
        }

       
    }

    void shoot()
    {
        Rigidbody bulletInstance = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        bulletInstance.velocity = transform.TransformDirection(new Vector3(0, 0, bulletSpd + 25));
    }
}

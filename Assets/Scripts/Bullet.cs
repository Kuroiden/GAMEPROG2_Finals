using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyBullet", 2.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DestroyOnContact());
    }

    IEnumerator DestroyOnContact()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}

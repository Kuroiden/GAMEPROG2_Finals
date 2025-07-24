using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAwakeTrigger : MonoBehaviour
{
    public AIManager aiManager;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aiManager.isAIAwake = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            aiManager.isAIAway = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aiManager.isAIAwake = false;
        }

        if (other.CompareTag("Enemy"))
        {
            aiManager.isAIAway = true;
        }
    }
}

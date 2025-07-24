using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool playerFound;
    public float timeCaught;
    public int counterVal;
    public Text counter;

    void Start()
    {
        timeCaught = 0;
        counterVal = 0;
        playerFound = false;
    }

    void Update()
    {
        if (playerFound) timeCaught += Time.deltaTime;

        counterVal = (int)timeCaught;

        counter.text = counterVal.ToString();
    }
}

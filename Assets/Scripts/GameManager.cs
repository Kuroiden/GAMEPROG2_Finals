using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;

    public GameObject Win;
    public GameObject Lose;
    public Text counter;

    public bool playerFound;
    public float timeCaught;
    public int counterVal;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        timeCaught = 0;
        counterVal = 0;
        playerFound = false;
    }

    void Update()
    {
        if (playerFound) timeCaught += Time.deltaTime;
        counterVal = (int)timeCaught;
        counter.text = counterVal.ToString();

        // Shows end screen and restarts game
        if (player.p_Win || player.p_Lose)
        {
            if (player.p_Win) Win.SetActive(true);
            else if (player.p_Lose) Lose.SetActive(true);

            if (Input.GetKey(KeyCode.R))
            {
                player.p_Lose = false;
                player.p_Win = false;

                counterVal = 0;

                SceneManager.LoadScene("Game Scene");
            }
        }
        else
        {
            Win.SetActive(false);
            Lose.SetActive(false);
        }
    }
}

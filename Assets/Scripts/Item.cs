using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Player player;
    public GameObject prompt;
    public TextMeshProUGUI promptText;
    public int itemType;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string action;

            if (itemType == 0) action = "Reload";
            else action = "Heal";

            prompt.SetActive(true);
            promptText.text = "<b>F</b>  " + action;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (Input.GetKey(KeyCode.F))
            {
                switch (itemType)
                {
                    case 0: // Ammo
                        player.Ammo += 10;
                        break;

                    case 1: // Medkit
                        player.HP = 5;
                        break;
                }

                this.gameObject.SetActive(false);
                prompt.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        prompt.SetActive(false);
    }
}

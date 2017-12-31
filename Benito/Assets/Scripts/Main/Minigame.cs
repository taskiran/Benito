using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour {

    //[HideInInspector]
    public int minigameID, spawnPositionID, minigameType;
    public bool upstairsMinigame;
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if ((player.isUpstairs && upstairsMinigame) || (!player.isUpstairs && !upstairsMinigame))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<BoxCollider>().enabled = true;
        }

        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}

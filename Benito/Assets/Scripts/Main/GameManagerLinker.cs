using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLinker : MonoBehaviour {

    [HideInInspector]
    public int minigamePlayingID = -1;

    private GameObject mainPlayer = null;
    private MainGameManager mainGameManager;

    public List<Vector3> posWithTuberias = new List<Vector3>();
    public List<Vector3> posWithPintar = new List<Vector3>();
    public List<GameObject> miniGames = new List<GameObject>();
    public uint[] numberOfMinigames;

    /*** AWAKE ***/
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManagerLinker").Length >= 2)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "main")
        {
            //mainPlayer = GameObject.FindGameObjectWithTag("Player");
            mainGameManager = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>();

            if (minigamePlayingID != -1)
            {
                mainGameManager.posWithTuberias = posWithTuberias;
                mainGameManager.posWithPintar = posWithPintar;
                mainGameManager.miniGames = miniGames;
                mainGameManager.numberOfMinigames = numberOfMinigames;

                mainGameManager.DestroyMiniGameAt(minigamePlayingID);
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

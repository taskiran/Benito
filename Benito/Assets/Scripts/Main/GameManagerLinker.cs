/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Enlazador entre los minijuegos y la escuela
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLinker : MonoBehaviour {

    [HideInInspector]
    public int minigamePlayingID = -1, minigameSpawnpositionID = -1, minigameType = -1, totalMinigames = -1;

    private GameObject mainPlayer = null;
    private MainGameManager mainGameManager;

    public List<Vector3> posWithTuberias = new List<Vector3>();
    public List<Vector3> posWithPintar = new List<Vector3>();
    public List<GameObject> miniGames = new List<GameObject>();
    public uint[] numberOfMinigames;
    public bool started = false;

    /*** AWAKE ***/
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManagerLinker").Length >= 2)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //if (SceneManager.GetActiveScene().name == "main")
        //{
        //    //mainPlayer = GameObject.FindGameObjectWithTag("Player");
        //    mainGameManager = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>();

        //    if (minigamePlayingID != -1)
        //    {
        //        print("A");
        //        mainGameManager.posWithTuberias = posWithTuberias;
        //        mainGameManager.posWithPintar = posWithPintar;
        //        mainGameManager.miniGames = miniGames;
        //        mainGameManager.numberOfMinigames = numberOfMinigames;

        //        //mainGameManager.DestroyMiniGameAt(minigamePlayingID);
        //    }
        //}
    }

 //   void Start () {
		
	//}
	
	void Update () {
        //print(miniGames.Count);
	}
}

/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Enlazador entre los minijuegos y la escuela
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

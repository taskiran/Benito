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
    public bool started = false, minigameCompleted = false, penDrivesCompleted = false;
    public uint numberOfMinigamesCompleted;
    public Vector3 playerPos;
    [HideInInspector]
    public bool _minigameCompleted, onMinigame;
    public bool completlyLinked, firstFloor;

    /*** AWAKE ***/
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManagerLinker").Length >= 2)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        completlyLinked = true;
        firstFloor = true;

        // BORRAME PRIMO //
        PlayerPrefs.SetInt("Day", 1);
    }

    private void Update()
    {
        if (minigameCompleted && !_minigameCompleted)
        {
            numberOfMinigamesCompleted++;
            _minigameCompleted = true;
        }
        
    }

}

/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Enlazador entre los minijuegos y la escuela
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

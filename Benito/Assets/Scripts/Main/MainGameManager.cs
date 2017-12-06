/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los minijuegos
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour {

    public GameObject[] miniGamesPrefabs;
    public float[] miniGamesProbabilities;
    public Transform[] tuberiasLocasSpawnPositions;
    public Transform[] pintarSpawnPositions;
    public float spawnTimer = 1f;
    public GameObject miniGamesParent;
    [Header("Maximo de minijuegos por cada minijuego")]
    public uint maxMiniGames = 2;

    private float _spawnTimer;
    private uint[] numberOfMinigames;
    private GameObject nearesMiniGame;
    private GameObject player;
    private List<GameObject> miniGames = new List<GameObject>();
    private GameObject arrow;
    private List<Vector3> posWithTuberias = new List<Vector3>();
    private List<Vector3> posWithPintar = new List<Vector3>();

    private uint mgn = 0;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        arrow = player.transform.GetChild(0).transform.gameObject;
        _spawnTimer = spawnTimer;
        numberOfMinigames = new uint[miniGamesPrefabs.Length];
	}
	
	// Update is called once per frame
	void Update () {
        _spawnTimer -= Time.deltaTime;
        if(_spawnTimer <= 0)
        {
            // Reinicia el temporizador
            _spawnTimer = spawnTimer;
            // Crea un minijuego
            GenerateMinigame();
        }

        if(miniGames.Count > 0)
        {
            Vector3 r = new Vector3(GetNearesMiniGame().transform.position.x, arrow.transform.position.y, GetNearesMiniGame().transform.position.z);
            arrow.transform.LookAt(r);
        }
        
	}

    void GenerateMinigame()
    {
        float r = Random.Range(0f, 1f);
        int i = -1;
        // Decide el objeto a instanciar
        foreach (float prob in miniGamesProbabilities)
        {
            int _i = System.Array.IndexOf(miniGamesProbabilities, prob);
            if (prob > r && numberOfMinigames[_i] < maxMiniGames)
            {
                i = _i;
            }
        }

        if(i == -1)
        {
            return;
        }

        // Crea y posiciona el objeto
        GameObject miniGame = miniGamesPrefabs[i];
        numberOfMinigames[i]++;
        Vector3 pos = Vector3.zero;
        // Posicion
        if(i == 0)
        {
            if(posWithTuberias.Count >= tuberiasLocasSpawnPositions.Length)
            {
                return;
            }
            bool posFinded = false;
            int p = 0;
            while (!posFinded)
            {
                p = Random.Range(0, tuberiasLocasSpawnPositions.Length);
                if (!posWithTuberias.Contains(tuberiasLocasSpawnPositions[p].transform.position))
                    posFinded = true;
            }
            pos = tuberiasLocasSpawnPositions[p].transform.position;
            posWithTuberias.Add(tuberiasLocasSpawnPositions[p].transform.position);
        }
        else if (i == 1)
        {
            if (posWithPintar.Count >= pintarSpawnPositions.Length)
            {
                return;
            }
            bool posFinded = false;
            int p = 0;
            while (!posFinded)
            {
                p = Random.Range(0, pintarSpawnPositions.Length);
                if (!posWithPintar.Contains(pintarSpawnPositions[p].transform.position))
                    posFinded = true;
            }
            pos = pintarSpawnPositions[p].transform.position;
            posWithPintar.Add(pintarSpawnPositions[p].transform.position);
        }
        GameObject inst = Instantiate(miniGame, pos, Quaternion.identity);
        inst.transform.parent = miniGamesParent.transform;
        inst.transform.name = "MiniGame n: " + mgn;
        mgn++;
        miniGames.Add(inst);
    }

    GameObject GetNearesMiniGame()
    {
        float nearestDistance = Vector3.Distance(player.transform.position, miniGames[0].transform.position);
        GameObject nearestObject = miniGames[0];
        foreach (var item in miniGames)
        {
            if(Vector3.Distance(player.transform.position, item.transform.position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(player.transform.position, item.transform.position);
                nearestObject = item;
            }
        }
        print(nearestObject.name);
        return nearestObject;
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los minijuegos
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

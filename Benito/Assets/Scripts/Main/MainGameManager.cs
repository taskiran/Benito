﻿/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los minijuegos
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour {

    /* Propiedades públicas */
    [Header("Gestión del día")]
    public float daySecondsDuration = 10f;
    public GameObject directionalLight;
    public Text dayTimerTxt;

    [Header("Gestión de minijuegos")]
    public GameObject[] miniGamesPrefabs;
    public float[] miniGamesProbabilities;
    public Transform[] tuberiasLocasSpawnPositions;
    public Transform[] pintarSpawnPositions;
    public float spawnTimer = 1f;
    [Header("Maximo de minijuegos por cada minijuego")]
    public uint maxMiniGames = 2;
    [Header("UI")]
    public GameObject goToMinigamePanel;
    public Text goToMinigameText;
    [HideInInspector]
    public int minigameToGoType;

    [Header("Interfaz")]
    public Image fadeImage;
    public float fadeTime = 2f;
    [HideInInspector]
    public bool fadeOut, onPanel;
    [HideInInspector]
    public string sceneToFadeName;

    /* Propiedades privadas */
    //-------------------Gestión de minijuegos------------------//
    private float _spawnTimer;
    [HideInInspector]
    public uint[] numberOfMinigames;
    private GameObject nearesMiniGame;
    private GameObject player;
    [HideInInspector]
    public List<GameObject> miniGames = new List<GameObject>();
    private GameObject arrow;
    [HideInInspector]
    public List<Vector3> posWithTuberias = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> posWithPintar = new List<Vector3>();
    private int totalMinigames;

    private uint mgn = 0;   // Nombre del minijuego a instanciar

    private GameManagerLinker linker;

    //-------------------Gestión el día-------------------------//
    private bool dayEnded = false;
    private float dayTimer = 0.0f;
    private float sunRotation;
    private float dayTimeNormalized;
    private Quaternion sunRotQuat;
    private float timeLeft;
    private float minLeft;
    private float secondsLeft;

    //-------------------Interfaz--------------------------------//
    private float fadeTimer = 0f;

    /*** START ***/
    void Start () {
        // Asignaciones iniciales
        fadeImage.gameObject.SetActive(false);
        goToMinigamePanel.SetActive(false);
        minigameToGoType = -1;
        dayTimer = 0.0f;
        fadeTimer = 0f;
        totalMinigames = 0;
        // Enlaces
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogError("Can't find player!");
        }
        linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
        arrow = player.transform.GetChild(0).transform.gameObject;
        _spawnTimer = spawnTimer;
        if(numberOfMinigames.Length == 0)
            numberOfMinigames = new uint[miniGamesPrefabs.Length];

        // Si se viene de un minijuego...
        if (linker.started)
        {
            totalMinigames = linker.totalMinigames;
            // Guarda las lista de posiciones de minijuegos y la lista de minijuegos con las que tiene el enlazador
            posWithTuberias = linker.posWithTuberias;
            posWithPintar = linker.posWithPintar;
            miniGames = linker.miniGames;
            numberOfMinigames = linker.numberOfMinigames;
            // Borra el minijuego del que se ha venido de todas las listas
            Destroy(miniGames[linker.minigamePlayingID]);
            miniGames.Remove(miniGames[linker.minigamePlayingID]);
            numberOfMinigames[linker.minigameType]--;
            if(linker.minigameType == 0)
                posWithTuberias.Remove(tuberiasLocasSpawnPositions[linker.minigameSpawnpositionID].transform.position);
            else if (linker.minigameType == 1)
                posWithPintar.Remove(tuberiasLocasSpawnPositions[linker.minigameSpawnpositionID].transform.position);
            totalMinigames--;
            print("Destroying pos at: " + linker.minigameSpawnpositionID);
        }
    }
	
	/*** UPDATE ***/
	void Update () {
        Day();
        Interface();
        MinigamesManager();
	}

    /*** Metodo para instanciar minijuegos y controlar la flecha ***/
    void MinigamesManager()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            // Reinicia el temporizador
            _spawnTimer = spawnTimer;
            // Crea un minijuego
            GenerateMinigame();
        }

        if (miniGames.Count > 0)
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
        Vector3 pos = Vector3.zero;
        // Posicion
        int p = 0;
        if (i == 0)
        {
            if(posWithTuberias.Count >= tuberiasLocasSpawnPositions.Length)
            {
                return;
            }
            bool posFinded = false;
            p = 0;
            while (!posFinded)
            {
                p = Random.Range(0, tuberiasLocasSpawnPositions.Length);
                if (!posWithTuberias.Contains(tuberiasLocasSpawnPositions[p].transform.position))
                    posFinded = true;
            }
            print("Instantiating at pos: " + p);
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
            p = 0;
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
        inst.transform.name = "MiniGame n: " + mgn;
        numberOfMinigames[i]++;
        mgn++;
        miniGames.Add(inst);
        inst.GetComponent<Minigame>().minigameID = totalMinigames;
        inst.GetComponent<Minigame>().spawnPositionID = p;
        inst.GetComponent<Minigame>().minigameType = i;
        totalMinigames++;
    }

    GameObject GetNearesMiniGame()
    {
        //print(miniGames[0].transform.position);
        //print(player.name);
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
        //print(nearestObject.name);
        return nearestObject;
    }
    
    void Day()
    {
        // Temporizador
        if(!dayEnded)
            dayTimer += Time.deltaTime;

        // Muestra el tiempo restante
        timeLeft = daySecondsDuration - dayTimer;
        minLeft = Mathf.Floor(timeLeft / 60);
        secondsLeft = (timeLeft % 60);
        dayTimerTxt.text = minLeft.ToString("00")+":"+secondsLeft.ToString("00");

        // Calcua el tiempo normalizado
        dayTimeNormalized = dayTimer / daySecondsDuration;
        // Calcula la rotación sol
        sunRotation = dayTimeNormalized * 190f;
        // Aplica la rotación del sol
        sunRotQuat = Quaternion.Euler(sunRotation, 0, 0);
        directionalLight.transform.rotation = sunRotQuat;

        // Si el tiempo se ha acabado, termina el dia
        if (dayTimer >= daySecondsDuration)
        {
            dayEnded = true;
        }
    }

    void Interface()
    {
        if(minigameToGoType != -1)
        {
            onPanel = true;
            goToMinigamePanel.SetActive(true);
            switch (minigameToGoType)
            {
                case (0):
                    goToMinigameText.text = "Tuberías Locas";
                    sceneToFadeName = "TuberíasLocas";
                    break;
                case (1):
                    goToMinigameText.text = "Humedades";
                    sceneToFadeName = "Humedades";
                    break;
            }
        }

        if (fadeOut)
        {
            fadeImage.gameObject.SetActive(true);

            fadeTimer += Time.deltaTime;

            Color col = fadeImage.color;
            col.a = fadeTimer / fadeTime;
            fadeImage.color = col;

            if(fadeTimer >= fadeTime)
            {
                minigameToGoType = -1;
                fadeOut = false;
                onPanel = false;

                linker.posWithTuberias = posWithTuberias;
                linker.posWithPintar = posWithPintar;
                linker.miniGames = miniGames;
                linker.numberOfMinigames = numberOfMinigames;
                linker.started = true;
                foreach (GameObject mg in miniGames)
                {
                    DontDestroyOnLoad(mg);
                }
                SceneManager.LoadScene(sceneToFadeName);
            }
        }
    }

    public void DestroyMiniGameAt(int id)
    {
        miniGames.RemoveAt(id);
    }

    /*** BUTTONS ***/
    public void GoToMinigame()
    {
        fadeOut = true;
        minigameToGoType = -1;
        goToMinigamePanel.SetActive(false);
    }
    public void CancelGoToMinigamePanel()
    {
        onPanel = false;
        minigameToGoType = -1;
        goToMinigamePanel.SetActive(false);
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los minijuegos
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

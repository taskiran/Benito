/*---------------------------------------------------------------------------
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
using UnityEngine.AI;

public class MainGameManager : MonoBehaviour {

    /* Propiedades públicas */
    [Header("Gestión del día")]
    public float daySecondsDuration = 10f;
    public GameObject directionalLight;
    public Text dayTimerTxt;
    [HideInInspector]
    public bool dayCompleted = false;

    [Header("Gestión de minijuegos")]
    public GameObject[] miniGamesPrefabs;
    public GameObject penDrivesPrefab;
    public float[] miniGamesProbabilities;
    public Transform[] tuberiasLocasSpawnPositions;
    public Transform[] pintarSpawnPositions;
    public Transform[] penDrivesSpawnPositions;
    public float spawnTimer = 1f;
    [Header("Maximo de minijuegos por cada minijuego")]
    public uint maxMiniGames = 2;
    [Header("Numero de minijuegos por dia")]
    public uint[] numberOfMinigamesPerDay;
    [Header("Alumnos")]
    public GameObject alumnoPrefab;
    public GameObject alumnoSpawnPosition;

    [Header("UI")]
    public GameObject goToMinigamePanel;
    public GameObject dayCompletedPanel;
    public GameObject gameCompletedPanel;
    public Text goToMinigameText;
    public Text dayText;
    public Text numberOfProblemsText;
    public Text numberOfPenDrivesText;
    public GameObject alumnoEnGaritaText;
    public GameObject gotoTuberias;
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
    [HideInInspector]
    public bool alumnoEnGarita = false;
    [HideInInspector]
    public uint numberOfAlumnosEnGarita = 0;
    [HideInInspector]
    public List<GameObject> alumnos = new List<GameObject>();

    private uint mgn = 0;   // Nombre del minijuego a instanciar

    private GameManagerLinker linker;

    private uint maxNumberOfMinigamesThisDay;

    //-------------------Gestión del día-------------------------//
    private bool dayEnded = false;
    private float dayTimer = 0.0f;
    private float sunRotation;
    private float dayTimeNormalized;
    private Quaternion sunRotQuat;
    private float timeLeft;
    private float minLeft;
    private float secondsLeft;
    [HideInInspector]
    public uint playerNumberOfPenDrives, totalNumberOfPenDrives;

    //-------------------Interfaz--------------------------------//
    private float fadeTimer = 0f;
    private Dialogos dialogs;

    /*** START ***/
    void Start () {
        SetUpDay();
        // Si se viene de un minijuego...
        if (linker.started && linker.minigameCompleted)
        {
            Link();
        }
        // Comprueba si ha acabado el dia
        if (linker.penDrivesCompleted)
        {
            // Indica que no se genere de nuevo el terreno
            GetComponent<GameGenerator>().dayStarted = true;
            // Gestion de pen drives
            //playerNumberOfPenDrives = linker.numberOfPenDrives;
            //playerNumberOfPenDrives += linker.numberOfPenDrivesPicked;
            linker.numberOfPenDrives += linker.numberOfPenDrivesPicked;
            // Acaba el dia
            dayCompleted = true;    
        }
            
    }
	
	/*** UPDATE ***/
	void Update () {
        if (!dayCompleted)
        {
            // Paneles
            gameCompletedPanel.SetActive(false);
            dayCompletedPanel.SetActive(false);

            if (GetComponent<GameGenerator>().sceneGenerated)
            {
                // Logica
                Day();
                Alumnos();
                Interface();
                MinigamesManager();
            }
            
        }
        else
        {   
            // Juego completado
            if(PlayerPrefs.GetInt("Day") == numberOfMinigamesPerDay.Length)
            {
                gameCompletedPanel.SetActive(true);
            }
            // Día completado
            else
            {
                dayCompletedPanel.SetActive(true);
            }
            
        }
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

    /*** Metodo para crear un minijuego ***/
    void GenerateMinigame()
    {
        float r = Random.Range(0f, 1f);
        int i = -1;
        // Decide el objeto a instanciar
        if(maxNumberOfMinigamesThisDay == linker.numberOfMinigamesCompleted)
        {
            i = 2;  // PenDrives
        }
        else
        {
            foreach (float prob in miniGamesProbabilities)
            {
                int _i = System.Array.IndexOf(miniGamesProbabilities, prob);
                if (prob > r && numberOfMinigames[_i] < maxMiniGames)
                {
                    i = _i;
                }
            }

            if (i == -1)
            {
                return;
            }
        }
        if (miniGames.Count + linker.numberOfMinigamesCompleted >= maxNumberOfMinigamesThisDay && i != 2) return;
        // Crea y posiciona el objeto
        GameObject miniGame = null;
        if (i == 2)
            miniGame = penDrivesPrefab;
        else
            miniGame = miniGamesPrefabs[i];
        Vector3 pos = Vector3.zero;
        // Posicion
        int p = 0;
        if (i == 0) // Tuberias
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
            pos = tuberiasLocasSpawnPositions[p].transform.position;
            posWithTuberias.Add(tuberiasLocasSpawnPositions[p].transform.position);

            numberOfMinigames[i]++;
        }
        else if (i == 1)    // Pintar
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

            numberOfMinigames[i]++;
        }
        else if (i == 2)    // PenDrives
        {
            p = 0;
            p = Random.Range(0, penDrivesSpawnPositions.Length);
            pos = penDrivesSpawnPositions[p].transform.position;
        }
        GameObject inst = Instantiate(miniGame, pos, Quaternion.identity);
        if(pos.y < 10f)
        {
            inst.GetComponent<Minigame>().upstairsMinigame = false;
        }
        else
        {
            inst.GetComponent<Minigame>().upstairsMinigame = true;
        }
        inst.transform.name = "MiniGame n: " + mgn;
        mgn++;
        if(i != 2)
            miniGames.Add(inst);
        inst.GetComponent<Minigame>().minigameID = totalMinigames;
        inst.GetComponent<Minigame>().spawnPositionID = p;
        inst.GetComponent<Minigame>().minigameType = i;
        totalMinigames++;
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
        return nearestObject;
    }
    
    /*** Metodo para la gestion del dia ***/
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
        // Si se han completado todos los minijuegos excepto los pen drives, fuerza la noche
        if (maxNumberOfMinigamesThisDay == linker.numberOfMinigamesCompleted)
        {
            sunRotQuat = Quaternion.Euler(190f, 0, 0);
        }
        directionalLight.transform.rotation = sunRotQuat;

        // Si el tiempo se ha acabado, termina el dia
        if (dayTimer >= daySecondsDuration)
        {
            dayEnded = true;
        }

        // Gestion de pen drives y alumnos
        if(numberOfAlumnosEnGarita == 0)
        {
            alumnoEnGarita = false;
        }
        else
        {
            alumnoEnGarita = true;
        }
        alumnoEnGaritaText.SetActive(alumnoEnGarita);

        // Muestra el dia por pantalla
        dayText.text = "Dia " + PlayerPrefs.GetInt("Day");
        // Muestra el numbero de estropicios por pantalla
        numberOfProblemsText.text = "Estropicios " + miniGames.Count;
        // Muestra el numero de pen drives por pantalla
        numberOfPenDrivesText.text = "PenDrives " + playerNumberOfPenDrives;
    }

    /*** Metodo para la gestion de los alumnos ***/
    void Alumnos()
    {
        if(linker.totalNumberOfPenDrives > 0)
        {
            GameObject alumno = Instantiate(alumnoPrefab, alumnoSpawnPosition.transform.position, Quaternion.identity);
            alumno.GetComponent<Alumno>().stairsPos = alumnoSpawnPosition.transform.position;
            linker.totalNumberOfPenDrives--;
            alumnos.Add(alumno);
        }
    }

    /*** Metodo para cargar escena segun el tipo de minijuego ***/
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
                    gotoTuberias.SetActive(true);
                    
                    break;
                case (1):
                    goToMinigameText.text = "Humedades";
                    sceneToFadeName = "Humedades";
                    break;
                case (2):
                    goToMinigameText.text = "PenDrives";
                    sceneToFadeName = "PenDrives";
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

                // Enlaza
                linker.posWithTuberias = posWithTuberias;
                linker.posWithPintar = posWithPintar;
                linker.miniGames = miniGames;
                linker.numberOfMinigames = numberOfMinigames;
                linker.started = true;
                linker.minigameCompleted = false;
                linker.onMinigame = true;
                linker.completlyLinked = false;
                linker.numberOfPenDrives = playerNumberOfPenDrives;
                linker.alumnos = alumnos;
                // No destruyas los minijuegos
                foreach (GameObject mg in miniGames)
                {
                    DontDestroyOnLoad(mg);
                }
                foreach (GameObject alumno in alumnos)
                {
                    DontDestroyOnLoad(alumno);
                    alumno.GetComponent<Alumno>().enabled = false;
                    alumno.GetComponent<NavMeshAgent>().enabled = false;
                    alumno.SetActive(false);
                }
                // Guarda el tiempo
                PlayerPrefs.SetFloat("DayTimer", dayTimer);

                // Carga el minijuego
                SceneManager.LoadScene(sceneToFadeName);
            }
        }
    }

    public void DestroyMiniGameAt(int id)
    {
        miniGames.RemoveAt(id);
    }

    /*** Metodo para iniciar el dia ***/
    void SetUpDay()
    {
        // Asignaciones iniciales
        fadeImage.gameObject.SetActive(false);
        goToMinigamePanel.SetActive(false);
        gotoTuberias.SetActive(false);
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
        dialogs = linker.gameObject.GetComponent<Dialogos>();
        arrow = player.transform.GetChild(0).transform.gameObject;
        _spawnTimer = spawnTimer;
        if (numberOfMinigames.Length == 0)
            numberOfMinigames = new uint[miniGamesPrefabs.Length];

        maxNumberOfMinigamesThisDay = numberOfMinigamesPerDay[PlayerPrefs.GetInt("Day") - 1];

        playerNumberOfPenDrives = linker.numberOfPenDrives;
    }

    /*** Metodo para enlazar al volver de un minijuego ***/
    void Link()
    {
        // Indica que no se genere de nuevo el terreno
        GetComponent<GameGenerator>().dayStarted = true;

        // Desactiva el colisionador del player
        player.GetComponent<BoxCollider>().enabled = false;

        // Esconde los paneles
        goToMinigamePanel.SetActive(false);

        // Activa el piso
        if (linker.firstFloor)
        {
            GetComponent<GameGenerator>().firstFloorTerrainTilesParent.SetActive(true);
            GetComponent<GameGenerator>().secondFloorTerrainTilesParent.SetActive(false);
            player.GetComponent<Player>().isUpstairs = false;
        }
        else
        {
            GetComponent<GameGenerator>().firstFloorTerrainTilesParent.SetActive(false);
            GetComponent<GameGenerator>().secondFloorTerrainTilesParent.SetActive(true);
            player.GetComponent<Player>().isUpstairs = true;
        }

        // Posiciona al player
        player.transform.position = linker.playerPos;

        // Gestion de alumnos //
        alumnos = linker.alumnos;
        foreach (GameObject alumno in alumnos)
        {
            alumno.GetComponent<Alumno>().enabled = true;
            alumno.GetComponent<NavMeshAgent>().enabled = true;
            
            alumno.SetActive(true);
        }

        // Gestion del minijuego //
        // Elimina el minijuego completado del mundo
        totalMinigames = linker.totalMinigames;
        Destroy(linker.miniGames[linker.minigamePlayingID]);
        linker.miniGames.RemoveAt(linker.minigamePlayingID);
        
        // Guarda las lista de posiciones de minijuegos y la lista de minijuegos con las que tiene el enlazador
        posWithTuberias = linker.posWithTuberias;
        posWithPintar = linker.posWithPintar;
        miniGames = linker.miniGames;
        numberOfMinigames = linker.numberOfMinigames;
        if (linker.minigameType == 0)
            posWithTuberias.Remove(tuberiasLocasSpawnPositions[linker.minigameSpawnpositionID].transform.position);
        else if (linker.minigameType == 1)
            posWithPintar.Remove(pintarSpawnPositions[linker.minigameSpawnpositionID].transform.position);
        // Reinicia el enlazador
        linker.totalMinigames = 0;
        linker.posWithTuberias.Clear();
        linker.posWithPintar.Clear();
        // Reinicia los indicies de los minijuegos ahora que son menos
        int i = 0;
        foreach (GameObject item in linker.miniGames)
        {
            item.SetActive(true);
            item.GetComponent<Minigame>().minigameID = i;
            i++;
        }
        totalMinigames = i;
        linker._minigameCompleted = false;
        linker.minigameCompleted = false;

        linker.onMinigame = false;
        linker.completlyLinked = true;

        // Activa el colisionador del player
        player.GetComponent<BoxCollider>().enabled = true;

        // Inicia el tiempo por donde estaba
        dayTimer = PlayerPrefs.GetFloat("DayTimer");

        // Esconde los paneles
        goToMinigamePanel.SetActive(false);
    }
    
    /*** BUTTONS ***/
    public void GoToMinigame()
    {
        fadeOut = true;
        minigameToGoType = -1;
        dialogs.enabled = false;
        goToMinigamePanel.SetActive(false);
    }
    public void CancelGoToMinigamePanel()
    {
        onPanel = false;
        minigameToGoType = -1;
        goToMinigamePanel.SetActive(false);
        gotoTuberias.SetActive(false);
    }
    public void NextDay()
    {
        PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day") + 1);
        linker.started = false;
        linker.numberOfMinigamesCompleted = 0;
        linker.numberOfPenDrivesPicked = 0;
        linker.minigameCompleted = false;
        linker.penDrivesCompleted = false;
        dayCompleted = false;
        GetComponent<GameGenerator>().dayStarted = false;
        GetComponent<GameGenerator>().sceneGenerated = false;
        SceneManager.LoadScene("main");
    }
    public void ResetStats()
    {
        PlayerPrefs.SetInt("Day", 1);
        PlayerPrefs.SetInt("TuberiasTutorial", 0);
        PlayerPrefs.SetInt("HumedadesTutorial", 0);
        PlayerPrefs.SetInt("PenDrivesTutorial", 0);
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los minijuegos
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

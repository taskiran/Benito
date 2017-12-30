/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador del jugador
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    /* Propiedades públicas */
    [Header("Player Stats")]
    public float movementSpeed = 10f;

    [Header("Gestión de camara")]
    public float farCameraOrtographicSize = 20f;
    public float changeCameraSizeSpeed = 10f;

    [Header("Escaleras")]
    public GameObject upStairs;
    public GameObject downStairs;

    [HideInInspector]
    public GameObject arrow;
    //[HideInInspector]
    public bool isUpstairs = false;

    /* Propiedades privadas */
    private GameGenerator generator;
    private NavMeshAgent agent;
    private Vector3 destination;
    public CinemachineVirtualCamera CMCamera;

    private bool farCamActive = false;
    private float startCameraOrtographicSize = 0f;

    private float ortographicSize;

    private MainGameManager gameManager;
    private GameManagerLinker linker;

    private bool isColliding = false;
    
	/*** Awake ***/
	void Awake () {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        generator = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<GameGenerator>();
        gameManager = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>();
        linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
        arrow = transform.GetChild(0).transform.gameObject;

        generator.firstFloorTerrainTilesParent.SetActive(true);
        generator.secondFloorTerrainTilesParent.SetActive(false);
    }

    /*** Start ***/
    private void Start()
    {
        // Posicion inicial
        agent.speed = movementSpeed;

        startCameraOrtographicSize = ortographicSize = CMCamera.m_Lens.OrthographicSize;

        
        destination = transform.position;
    }

    /*** Update ***/
    void Update () {
        // Movimiento
        if (!gameManager.fadeOut && !gameManager.onPanel)
        {
            agent.enabled = true;
            CheckMovementInput();
            Movement();
            isColliding = false;
        }
        else
        {
            agent.enabled = false;
        }
        

        // Gestion de camaras
        CameraManagement();
	}

    /*** Metodo para la gestion del input del jugador ***/
    void CheckMovementInput()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    print("Touching UI");
                }
                else
                {
                    RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.GetTouch(0).position));
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hit = hits[i];
                        if (hit.transform.gameObject.tag == "PintarTrigger"
                            ||
                            hit.transform.gameObject.tag == "TuberíasLocasTrigger")
                        {
                            destination = hit.transform.position;
                            break;
                        }
                        else if (hit.transform.gameObject.tag == "Tile")
                        {
                            destination = hit.transform.position;
                            break;
                        }
                    }
                }

            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.gameObject.tag == "PintarTrigger" 
                        ||
                        hit.transform.gameObject.tag == "TuberíasLocasTrigger")
                    {
                        destination = hit.transform.position;
                        break;
                    }
                    else if (hit.transform.gameObject.tag == "Tile")
                    {
                        destination = hit.transform.position;
                        break;
                    }
                }
            }
        }
    }

    /*** Metodo para el control de movimiento del player ***/
    void Movement()
    {
        float y = 0;
        if (isUpstairs)
        {
            y = 10.4f;
        }
        else
        {
            y = 1.5f;
        }
        Vector3 _destination = new Vector3(destination.x, y, destination.z);
        agent.SetDestination(_destination);
    }

    /*** Metodo para el manejo de la camara ***/
    void CameraManagement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            farCamActive = !farCamActive;
        }

        if (farCamActive)
        {
            ortographicSize = Mathf.Lerp(ortographicSize, farCameraOrtographicSize, changeCameraSizeSpeed * Time.deltaTime);
            CMCamera.m_Lens.OrthographicSize = ortographicSize;
        }
        else
        {
            ortographicSize = Mathf.Lerp(ortographicSize, startCameraOrtographicSize, changeCameraSizeSpeed * Time.deltaTime);
            CMCamera.m_Lens.OrthographicSize = ortographicSize;
        }

        Camera.main.orthographicSize = ortographicSize;

    }

    public void ChangeCameraFar()
    {
        farCamActive = !farCamActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TuberíasLocasTrigger")
        {
            gameManager.minigameToGoType = 0;
            linker.minigamePlayingID = other.transform.GetComponent<Minigame>().minigameID;
            linker.minigameSpawnpositionID = other.transform.GetComponent<Minigame>().spawnPositionID;
            linker.minigameType = other.transform.GetComponent<Minigame>().minigameType;
            linker.playerPos = transform.position;
            linker.firstFloor = other.GetComponent<Minigame>().upstairsMinigame ? false : true;
        }
        else if (other.tag == "PintarTrigger")
        {
            gameManager.minigameToGoType = 1;
            linker.minigamePlayingID = other.transform.GetComponent<Minigame>().minigameID;
            linker.minigameSpawnpositionID = other.transform.GetComponent<Minigame>().spawnPositionID;
            linker.minigameType = other.transform.GetComponent<Minigame>().minigameType;
            linker.playerPos = transform.position;
            linker.firstFloor = other.GetComponent<Minigame>().upstairsMinigame ? false : true;
        }
        else if (other.tag == "PenDrivesTrigger")
        {
            gameManager.minigameToGoType = 2;
            linker.minigamePlayingID = other.transform.GetComponent<Minigame>().minigameID;
            linker.minigameSpawnpositionID = other.transform.GetComponent<Minigame>().spawnPositionID;
            linker.minigameType = other.transform.GetComponent<Minigame>().minigameType;
            linker.playerPos = transform.position;
            linker.firstFloor = other.GetComponent<Minigame>().upstairsMinigame ? false : true;
        }
        else if (other.tag == "Stairs")
        {
            if (isColliding) return;
            isUpstairs = !isUpstairs;
            if (isUpstairs)
            {
                transform.position = upStairs.transform.position;
                generator.firstFloorTerrainTilesParent.SetActive(false);
                generator.secondFloorTerrainTilesParent.SetActive(true);
            }

            else
            {
                transform.position = downStairs.transform.position;
                generator.firstFloorTerrainTilesParent.SetActive(true);
                generator.secondFloorTerrainTilesParent.SetActive(false);
            }
            agent.enabled = false;
            destination = transform.position;
            agent.enabled = true;
            isColliding = true;
        }
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador del jugador
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

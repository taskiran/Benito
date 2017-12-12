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

    [HideInInspector]
    public GameObject arrow;

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
    
	/*** Awake ***/
	void Awake () {
        generator = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<GameGenerator>();
        gameManager = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>();
        linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
        agent = GetComponent<NavMeshAgent>();
        arrow = transform.GetChild(0).transform.gameObject;
	}

    /*** Start ***/
    private void Start()
    {
        destination = transform.position;
        agent.speed = movementSpeed;

        startCameraOrtographicSize = ortographicSize = CMCamera.m_Lens.OrthographicSize;
    }

    /*** Update ***/
    void Update () {
        // Movimiento
        if (!gameManager.fadeOut && !gameManager.onPanel)
        {
            CheckMovementInput();
            Movement();
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
        Vector3 _destination = new Vector3(destination.x, transform.position.y, destination.z);
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
            //gameManager.sceneToFadeName = "TuberíasLocas";
            gameManager.minigameToGoType = 0;
            linker.minigamePlayingID = other.transform.GetComponent<Minigame>().minigameID;
            linker.minigameSpawnpositionID = other.transform.GetComponent<Minigame>().spawnPositionID;
            linker.minigameType = other.transform.GetComponent<Minigame>().minigameType;
            //gameManager.fadeOut = true;
        }
        else if (other.tag == "PintarTrigger")
        {
            //gameManager.sceneToFadeName = "Humedades";
            gameManager.minigameToGoType = 1;
            linker.minigamePlayingID = other.transform.GetComponent<Minigame>().minigameID;
            linker.minigameSpawnpositionID = other.transform.GetComponent<Minigame>().spawnPositionID;
            linker.minigameType = other.transform.GetComponent<Minigame>().minigameType;
            //gameManager.fadeOut = true;
        }
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador del jugador
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

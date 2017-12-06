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

    [Header("Player Stats")]
    public float movementSpeed = 10f;

    [Header("Gestión de camara")]
    public float farCameraOrtographicSize = 20f;
    public float changeCameraSizeSpeed = 10f;

    private GameGenerator generator;
    private NavMeshAgent agent;
    private Vector3 destination;
    public CinemachineVirtualCamera CMCamera;

    private bool farCamActive = false;
    private float startCameraOrtographicSize = 0f;

    private float ortographicSize;
    [HideInInspector]
    public GameObject arrow;

	/*** Awake ***/
	void Awake () {
        generator = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameGenerator>();
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
        CheckMovementInput();
        Movement();

        // Gestion de camaras
        CameraManagement();
	}

    /*** Metodo para la gestion del input del jugador ***/
    void CheckMovementInput()
    {
        if(Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                destination = hit.transform.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.position != transform.position)
                {
                    destination = hit.transform.position;
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
            SceneManager.LoadScene(1);
        }
        else if (other.tag == "PintarTrigger")
        {
            SceneManager.LoadScene("Humedades");
        }
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador del jugador
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

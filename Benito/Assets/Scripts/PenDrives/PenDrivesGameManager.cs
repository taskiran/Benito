using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PenDrivesGameManager : MonoBehaviour {

    [Header("Enlaces")]
    public GameObject foco;
    public GameObject[] tablesParent;  // Objetos
    public GameObject canvas;
    [Header("Parametros del juego")]
    public float[] levelTime;   // Tiempo por nivel
    public float speed = 5f;
    public float focoSpeed = 5f;    // Velocidad al mover la "camara"
    public int[] numberOfTables;  // Numero de mesas por fila
    public float[] starTimes;
    [Header("Botones")]
    public Button leftBtn;
    public Button rightBtn;
    public Button upBtn;
    public Button downBtn;
    [HideInInspector]
    public uint penDrivesPicked;

    private bool moveLeft = false, moveRight = false, moveUp = false, moveDown = false;   // Flag para el movimineto
    private RectTransform tableTransform, focoTransform, parentTransform;   // Transform de los objetos
    private float xDest, yDest;    // Destino al que se dirige la "camara"
    private float xRightLimit, xLeftLimit;  // Limires de la "camara"
    private Vector3 focoDest;	// Destino del foco
    private bool moveFoco = false;	// Flag para mover el foco
	private int tableInd = 0;

    private int level = 0;  // Indice del nivel actual
    [HideInInspector]
    public int numberOfPenDrives = 4;   // Numero de pen drives en el minijuego
    private int numberOfTableRows;  // Numero de filas de mesas
    private GameObject actualTable;

    private bool win, gameOver; // Flags para el estado del juego
    private PenDrivesGUI gui;	// GUI
    private GameManagerLinker linker;   // Linker;
    private Dialogos dialogs;   // Dialogos

    [HideInInspector]
    public float starTimeToArchive = 0.0f, totalTimer = 0.0f;

    // Use this for initialization
    void Start () {
        gui = GetComponent<PenDrivesGUI>();
        if (GameObject.FindGameObjectWithTag("GameManagerLinker"))
        {
            linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
            dialogs = linker.gameObject.GetComponent<Dialogos>();
        }
            
        //PlayerPrefs.SetInt("Day", 1);
        // Enlaces y valores iniciales
        switch (PlayerPrefs.GetInt("Day"))
        {
            case 1: // Dia 1
                level = Random.Range(0, 2);
                gui.totalTime = levelTime[level];
                break;
            case 2: // Dia 2
                level = Random.Range(0, 2);
                gui.totalTime = levelTime[level];
                break;
        }
        focoTransform = foco.GetComponent<RectTransform>();
        win = gameOver = false;

        starTimeToArchive += starTimes[level];

        LevelSetUp();
	}
	
	// Update is called once per frame
	void Update () {
        // Si el juego no ha acabado...
        if (!gui.ended && !dialogs.onDialog)
        {
            // Temporizador
            totalTimer += Time.deltaTime;
            // Si el foco no se mueve...
            if (!moveFoco)
            {
                // Movimiento de las mesas
                TableMovement();
            }
            // Si la mesa no se mueve...
            if(!moveLeft && !moveRight && !moveUp && !moveDown)
            {
                // Foco
                Foco();
            }
            
        }
	}

    void TableMovement()
    {
		
		if (moveLeft) {
            parentTransform.Translate (Vector3.right * speed * Time.deltaTime);
			if (parentTransform.localPosition.x >= xDest) {
                parentTransform.localPosition = new Vector3 (xDest, yDest, 0);
				moveLeft = false;
			}
		}
		else if (moveRight) {
            parentTransform.Translate (Vector3.left * speed * Time.deltaTime);
			if (parentTransform.localPosition.x <= xDest) {
                parentTransform.localPosition = new Vector3 (xDest, yDest, 0);
				moveRight = false;
			}
		}
		else if (moveUp) {
            parentTransform.Translate (Vector3.up * speed * Time.deltaTime);
			if (parentTransform.localPosition.y >= yDest) {
                parentTransform.localPosition = new Vector3 (xDest, yDest, 0);
				moveUp = false;
			}
		}
		else if (moveDown) {
            parentTransform.Translate (Vector3.down * speed * Time.deltaTime);
			if (parentTransform.localPosition.y <= yDest) {
                parentTransform.localPosition = new Vector3(xDest, yDest, 0);
                moveDown = false;
			}
		}

        // Limites
        if (parentTransform.localPosition.x > xLeftLimit)
        {
            parentTransform.localPosition = new Vector3(xLeftLimit, parentTransform.localPosition.y, 0);
        }
        else if (parentTransform.localPosition.x < 0)
        {
            parentTransform.localPosition = new Vector3(0, parentTransform.localPosition.y, 0);
        }

        // Movimiento horizontal
        if (Input.GetKeyDown(KeyCode.A) && !moveLeft && !moveRight && parentTransform.localPosition.x != xLeftLimit && !moveUp && !moveDown)
        {
            xDest = parentTransform.localPosition.x + 1020f;
            moveLeft = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !moveLeft && !moveRight && parentTransform.localPosition.x != xRightLimit && !moveUp && !moveDown)
        {
            xDest = parentTransform.localPosition.x - 1020f;
            moveRight = true;
        }
        // Movimiento vertical
        else if (Input.GetKeyDown(KeyCode.W) && tableInd != 0 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            yDest = parentTransform.localPosition.y - 630;
            moveDown = true;
            tableInd--;
        }
        else if (Input.GetKeyDown(KeyCode.S) && tableInd != numberOfTableRows - 1 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            yDest = parentTransform.localPosition.y + 630;
            moveUp = true;
            tableInd++;
        }

        // Botones
        Buttons();
    }
    void Foco()
    {
        if (moveFoco)
        {
            Vector3 dir = focoDest - focoTransform.localPosition;
            float distance = dir.magnitude;
            dir = dir / distance;
            focoTransform.Translate(dir * focoSpeed * Time.deltaTime);
            float dist = Vector3.Distance(focoTransform.localPosition, focoDest);
            if(dist < 3.5f)
            {
                moveFoco = false;
                foco.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                foco.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        else
        {
            focoTransform.localPosition = new Vector3(560, 0, 0);
            foco.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void LevelSetUp()
    {
        if (actualTable != null) Destroy(actualTable);
        GameObject t = Instantiate(tablesParent[level]);
        t.transform.parent = canvas.transform;
        t.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        t.GetComponent<RectTransform>().localPosition = Vector3.zero;
        t.transform.SetSiblingIndex(0);
        actualTable = t;

        parentTransform = actualTable.GetComponent<RectTransform>();
        numberOfPenDrives = 0;
        numberOfTableRows = 0;
        foreach (Transform obj in actualTable.transform)
        {
            if (obj.gameObject.tag == "PenDrive")
            {
                EventTrigger trigger = obj.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { PointerClick(); });
                trigger.triggers.Add(entry);

                numberOfPenDrives++;
            }
            else if (obj.gameObject.tag == "Table")
            {
                EventTrigger trigger = obj.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { PointerClick(); });
                trigger.triggers.Add(entry);

                numberOfTableRows++;
            }
            else if (obj.gameObject.tag == "Obstaculo")
            {
                EventTrigger trigger = obj.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { PointerClick(); });
                trigger.triggers.Add(entry);
            }
        }
        gui.penDrivesLeft = numberOfPenDrives;
        linker.totalNumberOfPenDrives += (uint)numberOfPenDrives;

        // Limites
        xRightLimit = 0;
        xLeftLimit = (1020 * numberOfTables[level]) - 1020;
    }

    void Buttons()
    {
        // Derecha
        if (!moveLeft && !moveRight && parentTransform.localPosition.x != xRightLimit && !moveUp && !moveDown)
        {
            rightBtn.interactable = true;
        }
        else
        {
            rightBtn.interactable = false;
        }

        // Izquierda
        if (!moveLeft && !moveRight && parentTransform.localPosition.x != xLeftLimit && !moveUp && !moveDown)
        {
            leftBtn.interactable = true;
        }
        else
        {
            leftBtn.interactable = false;
        }

        // Arriba
        if (tableInd != 0 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            upBtn.interactable = true;
        }
        else
        {
            upBtn.interactable = false;
        }

        // Abajo
        if (tableInd != numberOfTableRows - 1 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            downBtn.interactable = true;
        }
        else
        {
            downBtn.interactable = false;
        }
    }

    public void ReturnToWorld()
    {
        if (linker)
        {
            linker.penDrivesCompleted = true;
            linker.numberOfPenDrivesPicked = penDrivesPicked;
        }
        SceneManager.LoadScene("main");
    }

    public void PointerClick()
    {
        if (!moveLeft && !moveRight && !moveUp && !moveDown)
        {
            focoDest = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 0);
            moveFoco = true;
        }
        
    }

    /*** Botones ***/
    public void GoRight()
    {
        if (!moveLeft && !moveRight && parentTransform.localPosition.x != xRightLimit && !moveUp && !moveDown)
        {
            xDest = parentTransform.localPosition.x - 1020f;
            moveRight = true;
        }
    }
    public void GoLeft()
    {
        if (!moveLeft && !moveRight && parentTransform.localPosition.x != xLeftLimit && !moveUp && !moveDown)
        {
            xDest = parentTransform.localPosition.x + 1020f;
            moveLeft = true;
        }
    }
    public void GoUp()
    {
        if (tableInd != 0 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            yDest = parentTransform.localPosition.y - 630;
            moveDown = true;
            tableInd--;
        }
    }
    public void GoDown()
    {
        if (tableInd != numberOfTableRows - 1 && !moveUp && !moveDown && !moveLeft && !moveRight)
        {
            yDest = parentTransform.localPosition.y + 630;
            moveUp = true;
            tableInd++;
        }
    }
}

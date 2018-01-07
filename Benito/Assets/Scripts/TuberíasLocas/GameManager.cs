/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de juego de Tuberías Locas
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    /* Propiedades publicas */
    public float waterSpeed = 0.5f;
    public float waterSpeedOverTime = 0.1f;
    public GameObject winObj;
    public GameObject gameOverObj;
    public GameObject goToWorldObj;
    [HideInInspector]
    public bool win = false, gameOver = false;
    public Dialogos dialogs;
    public float giveStarsTime = 0.5f;

    /* Propiedades privadas */
    private float counter = 0f;
    private bool count = false;
    private GameObject pipe;
    private Generator generator;
    private float startSpeed = 0.5f;
    private GameManagerLinker linker;
    private bool loosed = false;
    private float timeLoosed = 0.0f;
    [HideInInspector]
    public float twoStarsTimeArchivement = 0.0f, threeStarsTimeArchivement = 0.0f;

    private int starsToGive = 0, starsGived = 0;
    private bool starsCalculated = false;

    private void Awake()
    {
        generator = GetComponent<Generator>();
        if (GameObject.FindGameObjectWithTag("GameManagerLinker"))
        {
            linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
            dialogs = linker.gameObject.GetComponent<Dialogos>();
        }
        
    }

    private void Start()
    {
        winObj.SetActive(false);
        gameOverObj.SetActive(false);
        goToWorldObj.SetActive(false);
        startSpeed = waterSpeed;
        loosed = false;
    }

    // Update is called once per frame
    void Update () {
        // Bucle de juego sin ganar
        if (!win && !gameOver && !dialogs.onDialog)
        {
            GetPlayerInput();
            ManageWater();
            timeLoosed += Time.deltaTime;
            //print(twoStarsTimeArchivement + ", " + timeLoosed);
        }
            
        // Bucle de juego una vez ganado
        if (win)
            Win();

        // Bucle de juego una vez perdido
        if (gameOver)
            GameOver();

        // BORRAME BRO
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            win = true;
        }
	}

    /* Controlar el agua */
    void ManageWater()
    {
        waterSpeed += waterSpeedOverTime * Time.deltaTime;
    }

    /* Input del jugador */
    void GetPlayerInput()
    {
        if (Application.isMobilePlatform)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero);

                if (hit && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    hit.transform.GetComponent<Pipe>().ChangePipe();
                    pipe = hit.transform.gameObject;
                    count = true;
                }
                if (hit && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    pipe = null;
                    count = false;
                    counter = 0;
                }
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit && Input.GetMouseButtonDown(0))
            {
                hit.transform.GetComponent<Pipe>().ChangePipe();
                pipe = hit.transform.gameObject;
                count = true;
            }
            if (hit && Input.GetMouseButtonDown(0))
            {
                pipe = null;
                count = false;
                counter = 0;
            }
        }
        

        if (count)
        {
            counter += Time.deltaTime;

            if (counter >= 1f)
            {
                pipe.GetComponent<Pipe>().ChangePipeType();
                count = false;
                counter = 0;
            }
        }
    }

    /* Acelerar agua */
    public void SpeedUp()
    {
        waterSpeed = waterSpeed * 10;
    }

    /* Reiniciar parametros */
    void ResetStats()
    {
        win = false;
        gameOver = false;
        winObj.SetActive(false);
        gameOverObj.SetActive(false);
        waterSpeed = startSpeed;
    }

    /* Siguiente nivel */
    public void NextLevel()
    {
        ResetStats();
        generator.levelsCompleted++;
        generator.SetRandomlevel();
        generator.DestroyStage();
        generator.GenerateStage();
    }

    /* Ganar */
    void Win()
    {
        if(generator.levelsCompleted == 1)
        {
            goToWorldObj.SetActive(true);
            if(!starsCalculated)
                CalculateSatrs();
            if(!IsInvoking("GiveStars"))
                InvokeRepeating("GiveStars", 0, giveStarsTime);
        }
        else
        {
            winObj.SetActive(true);
        }
    }

    /* Calcula el numero de estrellas a dar */
    void CalculateSatrs()
    {
        // No ha perdido = 1 estrella
        if (!loosed)
        {
            starsToGive++;
        }
        // Ha completado el juego en x tiempo = 2 estrellas
        if (timeLoosed < twoStarsTimeArchivement)
        {
            starsToGive++;
        }
        // Ha completado el juego en x tiempo = 3 estrellas
        if (timeLoosed < threeStarsTimeArchivement)
        {
            starsToGive++;
        }

        starsCalculated = true;
    }

    /* Dar estrellas */
    void GiveStars()
    {
        if (starsToGive > 0)
        {
            int starIndex = starsGived + 2;
            goToWorldObj.transform.GetChild(starIndex).gameObject.SetActive(true);
            goToWorldObj.transform.GetChild(starIndex).transform.GetChild(0).GetComponent<Animator>().Play("IntroduceStar" + (starIndex - 1));
            starsToGive--;
            starsGived++;
        }
        else
        {
            CancelInvoke("GiveStars");
        }
    }

    /* Volver a intentar */
    public void TryAgain()
    {
        ResetStats();
        generator.DestroyStage();
        generator.GenerateStage();
    }

    /* Perder */
    void GameOver()
    {
        gameOverObj.SetActive(true);
        loosed = true;
    }

    /* Vuelve al mundo */
    public void GoToWorld()
    {
        if (linker != null)
        {
            if(win)
                linker.minigameCompleted = true;
        }
        
        SceneManager.LoadScene("main");
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de juego
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

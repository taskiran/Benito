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

    /* Propiedades privadas */
    private float counter = 0f;
    private bool count = false;
    private GameObject pipe;
    private Generator generator;
    private float startSpeed = 0.5f;
    private GameManagerLinker linker;

    private void Awake()
    {
        generator = GetComponent<Generator>();
        if(GameObject.FindGameObjectWithTag("GameManagerLinker"))
            linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
    }

    private void Start()
    {
        winObj.SetActive(false);
        gameOverObj.SetActive(false);
        goToWorldObj.SetActive(false);
        startSpeed = waterSpeed;
    }

    // Update is called once per frame
    void Update () {
        // Bucle de juego sin ganar
        if (!win && !gameOver)
        {
            GetPlayerInput();
            ManageWater();
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
        }
        else
        {
            winObj.SetActive(true);
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
    }

    /* Vuelve al mundo */
    public void GoToWorld()
    {
        if (linker != null) linker.minigameCompleted = true;
        SceneManager.LoadScene("main");
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de juego
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

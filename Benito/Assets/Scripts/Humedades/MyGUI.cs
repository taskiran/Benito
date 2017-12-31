using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGUI : MonoBehaviour {

    public GameObject gameOverObj, winObj, timerObj, pinturaIndicador, peligroIndicador;
    [HideInInspector]
    public bool gameOver = false, win = false, next = false;
    [HideInInspector]
    public float startTime;

    private HumedadesGameManager manager;
    [HideInInspector]
    public float timer, timeLeft, secondsLeft, pinturaPercent, peligroPercent;

    private float timePercent;

	// Use this for initialization
	void Start () {
        gameOverObj.SetActive(false);
        winObj.SetActive(false);
        manager = GetComponent<HumedadesGameManager>();
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (gameOver)
        {
            gameOverObj.SetActive(true);
        }
        else
        {
            gameOverObj.SetActive(false);
        }

        if (win)
        {
            winObj.SetActive(true);
            winObj.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "GoToWorld";
            winObj.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            winObj.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(manager.GoToWorld);
        }
        else if (next)
        {
            winObj.SetActive(true);
            winObj.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "Next Level";
            winObj.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            winObj.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(manager.TryAgain);
        }
        else
        {
            winObj.SetActive(false);
        }

        if(!gameOver && !win && !next)
        {
            // Tiempo
            timeLeft = startTime - timer;
            secondsLeft = (timeLeft % 60);
            timePercent = secondsLeft / manager.time;
            if (secondsLeft <= 0)
            {
                gameOver = true;
                manager.gameOver = true;
            }
            else
            {
                startTime -= Time.deltaTime;
                timerObj.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = timePercent;
            }
        }

        // Indicadores
        pinturaIndicador.GetComponent<Image>().fillAmount = pinturaPercent;
        peligroIndicador.GetComponent<Image>().fillAmount = peligroPercent;

    }
}

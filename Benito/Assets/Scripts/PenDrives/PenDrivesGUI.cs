using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenDrivesGUI : MonoBehaviour {

	public Text penDrivesLeftTxt;
    public GameObject timeObj;
    public GameObject returnToWorldPanel;
    public float giveStarsRate = 0.5f;
	[HideInInspector]
	public int penDrivesLeft;
    [HideInInspector]
    public bool ended;
    [HideInInspector]
    public float totalTime;

    private float timeLeft, timer, timePercent;

    private int starsToGive = 0, starsGived = 0;
    private bool starsCalculated = false;

    private PenDrivesGameManager gm;
    private Dialogos dialogs;
	
	// Use this for initialization
	void Start () {
        returnToWorldPanel.SetActive(false);
        timer = 0;
        gm = GetComponent<PenDrivesGameManager>();
        dialogs = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<Dialogos>();
    }
	
	// Update is called once per frame
	void Update () {
        //print(timeLeft);
        if (ended)
        {
            returnToWorldPanel.SetActive(true);
            if(!starsCalculated)
                CalculateStars();
            if (!IsInvoking("GiveStars"))
                InvokeRepeating("GiveStars", 0, giveStarsRate);
        }

        if (penDrivesLeft <= 0)
        {
            ended = true;
        }

        penDrivesLeftTxt.text = "Quedan: " + penDrivesLeft;

        if (!ended && !dialogs.onDialog)
        {
            // Tiempo
            timeLeft = totalTime - timer;
            timePercent = timeLeft / totalTime;
            if (timeLeft <= 0)
            {
                ended = true;
            }
            else
            {
                timer += Time.deltaTime;
                timeObj.GetComponent<Image>().fillAmount = timePercent;
            }
        }
        
    }

    /* Calcular las estrellas a dar */
    void CalculateStars()
    {
        // 1 pen drive encontrado = 1 estrella
        if (gm.penDrivesPicked > 0)
            starsToGive++;
        // Ha completado el juego en x tiempo = 2 estrellas
        if (gm.totalTimer < gm.starTimeToArchive)
            starsToGive++;
        // Ha encontrado todos los pen drives = 3 estrellas
        if (gm.numberOfPenDrives == gm.penDrivesPicked)
            starsToGive++;

        starsCalculated = true;
    }

    /* Dar las estrellas */
    void GiveStars()
    {
        if (starsToGive > 0)
        {
            int starIndex = starsGived + 2;
            returnToWorldPanel.transform.GetChild(starIndex).gameObject.SetActive(true);
            returnToWorldPanel.transform.GetChild(starIndex).transform.GetChild(0).GetComponent<Animator>().Play("IntroduceStar" + (starIndex - 1));
            starsToGive--;
            starsGived++;
        }
        else
        {
            CancelInvoke("GiveStars");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenDrivesGUI : MonoBehaviour {

	public Text penDrivesLeftTxt;
    public GameObject timeObj;
    public GameObject returnToWorldPanel;
	[HideInInspector]
	public int penDrivesLeft;
    [HideInInspector]
    public bool ended;
    [HideInInspector]
    public float totalTime;

    private float timeLeft, timer, timePercent;
	
	// Use this for initialization
	void Start () {
        returnToWorldPanel.SetActive(false);
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //print(timeLeft);
        if (ended)
        {
            returnToWorldPanel.SetActive(true);
        }

        if (penDrivesLeft <= 0)
        {
            ended = true;
        }

        penDrivesLeftTxt.text = "Quedan: " + penDrivesLeft;

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

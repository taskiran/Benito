using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foco : MonoBehaviour {

	public PenDrivesGUI gui;	// GUI

    private PenDrivesGameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PenDrivesGameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.tag == "PenDrive") {
			gui.penDrivesLeft--;
            gm.penDrivesPicked++;
            Destroy(collision.gameObject);
		}
    }
}

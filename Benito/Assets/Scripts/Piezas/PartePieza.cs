using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartePieza : MonoBehaviour {

	private bool mouseClickedThisObject = false;
	private RectTransform rect;
	private Transform childTransform;
	private Vector3 startPos;

	// Use this for initialization
	void Start () {
		rect = GetComponent<RectTransform>();
		childTransform = transform.GetChild(0);
		startPos = rect.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(!PiezasGameManager.win && !PiezasGameManager.gameOver){
			Behaviour();
		}
	}

	void Behaviour(){
		if(Input.GetMouseButton(0) && mouseClickedThisObject){
			Move();
		}
		else if(Input.GetMouseButtonUp(0)){
			mouseClickedThisObject = false;
			PiezasGameManager.holdingObject = null;
		}
		else{
			Rotate();
			rect.position = startPos;
		}
	}

	void Move(){
		Vector3 mouse = Input.mousePosition;
		mouse.z = 0;
		rect.anchoredPosition = Input.mousePosition;
		PiezasGameManager.holdingObject = gameObject.transform.GetChild(0).gameObject;
	}
	void Rotate(){
		childTransform.Rotate(new Vector3(1,0,1) * 10.0f * Time.deltaTime);
	}

	void OnMouseDown(){
		mouseClickedThisObject = true;
	}
	void OnMouseUp(){
		
	}
}

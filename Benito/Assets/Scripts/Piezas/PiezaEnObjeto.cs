using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezaEnObjeto : MonoBehaviour {

	public Material completedMat;
	[HideInInspector]
	public Material highlightedMaterial;
	[HideInInspector]
	public bool onOver = false, completed = false;

	private Renderer rend;
	private Material normalMat;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		normalMat = rend.material;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!completed){
			rend.material = onOver ? highlightedMaterial : normalMat;
			onOver = false;
		}
		else{
			rend.material = completedMat;
		}

	}
}

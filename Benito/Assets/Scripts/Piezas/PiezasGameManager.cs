using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezasGameManager : MonoBehaviour {

	[Header("Generación de juego")]
	public PiezasJuegoSO[] games;

	[Header("Juego")]
	public static GameObject holdingObject;
	public Material highlightedMaterial;
	public float jigglingTime = 0.2f;
	public uint maxTimesFail = 3;

	public static bool win, gameOver;

	private Ray ray;
	private RaycastHit hitInfo;
	private Camera cam;
	private int piezaLayer;
	private string highlightedPiezaName;

	private GameObject mainObject, piezasParent;
	private int holesToFill = 0; 	// 8===D

	private GameObject canvas;

	private float jigglingTimer = 0.0f;
	private bool jiggling = false;
	private uint timesFailed = 0;
	// Use this for initialization
	void Start () {
		SetUpGame();
		mainObject = GameObject.Find("MainObject").transform.GetChild(0).gameObject;
		timesFailed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		InGame();
	}

	void SetUpGame(){
		// Crea el escenario
		GameObject mainObject = Instantiate(games[0].piezaPrincipal, Vector3.zero, Quaternion.identity);
		mainObject.name = "MainObject";
		canvas = GameObject.Find("Canvas");
		GameObject piezas = Instantiate(games[0].piezasCanvas);
		piezas.transform.parent = canvas.transform;
		RectTransform piezasRect = piezas.GetComponent<RectTransform>();
		piezasRect.localPosition = new Vector3(1024 / 2, 600 / 2, 0);
		piezasRect.anchorMin = Vector2.zero;
		piezasRect.anchorMax = Vector2.zero;
		piezasRect.localScale = Vector3.one;
		piezasRect.sizeDelta = new Vector2(1024, 600);

		piezasParent = piezas;

		// Configura el juego
		cam = Camera.main;
		piezaLayer |= (1 << 8);
		holdingObject = null;

		mainObject = GameObject.Find("MainObject").transform.GetChild(0).gameObject;
		for(int i = 0; i < mainObject.transform.childCount; i++){
			if(mainObject.transform.GetChild(i).gameObject.layer == 8){
				holesToFill++;
			}
		}
	}

	void InGame(){
		// Si esta sujetando un objeto...
		if(holdingObject != null && !win && !gameOver){
			// Lanza un rayo desde el raton
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Si encuentra un agujero en el objeto principal...
			if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, piezaLayer)){
				// Indica al agujero que se muestre
				hitInfo.transform.gameObject.GetComponent<PiezaEnObjeto>().onOver = true;
				hitInfo.transform.gameObject.GetComponent<PiezaEnObjeto>().highlightedMaterial = highlightedMaterial;
				highlightedPiezaName = hitInfo.transform.gameObject.name;
				// Si suelta la pieza estando encima de un agujero...
				if(Input.GetMouseButtonUp(0)){
					// Si es la pieza que va a ese agujero...
					if(highlightedPiezaName == holdingObject.name){
						// Indica al agujero que se rellene (cambie material)
						hitInfo.transform.gameObject.GetComponent<PiezaEnObjeto>().completed = true;
						// Destruye el objeto que sujeta el player
						Destroy(holdingObject.transform.parent.gameObject);
						holdingObject = null;
						// Indica que queda 1 agujero menos a que rellenar
						holesToFill--;
						hitInfo.transform.gameObject.layer = 0;
						if(holesToFill <= 0){
							win = true;
						}
					}
					else{
						jiggling = true;
						timesFailed++;
					}
				}
			}
		}

		if(jiggling){
			mainObject.transform.position = new Vector3(Mathf.Sin(Time.time * 35.0f) * 0.2f, mainObject.transform.position.y, mainObject.transform.position.z);
			jigglingTimer += Time.deltaTime;
			if(jigglingTimer >= jigglingTime){
				mainObject.transform.position = Vector3.zero;
				jigglingTimer = 0.0f;
				jiggling = false;
				if(timesFailed >= maxTimesFail)
					gameOver = true;
			}
		}

		if(gameOver)
			GameOver();
		if(win)
			Win();
	}

	void GameOver(){
		for (int i = 0; i < mainObject.transform.childCount; i++){
			mainObject.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	void Win(){
		print("Win!");
	}
}

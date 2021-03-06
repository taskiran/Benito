﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HumedadesGameManager : MonoBehaviour {

    [Header("Enlaces")]
    public GameObject brush, brushContainer;    // Prefab de la brocha a instanciar y contenedor de los brushed;
    public RenderTexture canvasTexture; // Textura de renderizado que se convierte al material del modelo (plano)
    public Material baseMaterial;   // Material del plano frente a la camara de renderizado de textura
    public Texture2D mainTex;   // Textura del plano frente a la camara de renderizado de textura
    public Material startMaterial;

    public Slider whiteSlider;  // % de pintura blanca
    public Slider redSlider;    // % de zona roja pintada

    [Header("Parametros del juego")]
    public GameObject brocha;
    public Texture2D[] texturesToPaint;
    public float[] threeStarsTimes;
    public float[] oneStarDangerZones;
    public float redPToLoose = 30.0f;
    public float pinturaLoosePerPaint = 10.0f;
    public uint levelsToComplete = 2;
    public float time = 99999f;
    public float giveStarsTime = 0.5f;

    [HideInInspector]
    public uint levelsCompleted = 0;

    private uint level = 0;

    private int brushesInstances = 0;   // Controlador de instancias de brochas para fusionar
    private float fullRedP = 0;

    [HideInInspector]
    public bool gameOver = false;

    private MyGUI gui;

    private float pinturaLoosed;

    private GameManagerLinker linker;
    private Dialogos dialogs;

    private float totalTimeTimer = 0.0f;
    private float threeStarsTimer = 0.0f;

    private float totalDangerZones = 0.0f;
    private float dangerZones = 0.0f;

    private bool loosed = false;

    private int totalStars = 0;
    private int starsGived = 0;

    private bool starsCalculated;

	// Use this for initialization
	void Start () {
        // Enlaces
        brush.GetComponent<SpriteRenderer>().color = Color.white;
        if (GameObject.FindGameObjectWithTag("GameManagerLinker"))
        {
            linker = GameObject.FindGameObjectWithTag("GameManagerLinker").GetComponent<GameManagerLinker>();
            dialogs = linker.gameObject.GetComponent<Dialogos>();
        }
            
        gameOver = false;
        gui = this.GetComponent<MyGUI>();
        if (gui == null) Debug.LogError("GUI not finded!");
        levelsCompleted = 0;
        gui.startTime = time;
        pinturaLoosed = 0f;

        totalTimeTimer = 0.0f;
        threeStarsTimer = 0.0f;

        totalDangerZones = 0.0f;
        dangerZones = 0.0f;

        loosed = false;

        SelectTextureToPaint();

        CheckRedFirstTime();
        
    }
	
	// Update is called once per frame
	void Update () {

        if (!gameOver && !gui.win && !gui.next && !dialogs.onDialog)
        {
            Game();
        }

        
        
        // BORRAME
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Win();
        }
    }

    void Game()
    {
        // Temporizador
        totalTimeTimer += Time.deltaTime;

        // Variables
        Vector3 uvWorldPosition = Vector3.zero;
        GameObject _brush = null;

        // Si se clicka sobre el plano, instancia una brocha
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            _brush = Instantiate(brush);
            _brush.transform.parent = brushContainer.transform;
            _brush.transform.localPosition = uvWorldPosition;

            brushesInstances++;
            pinturaLoosed += pinturaLoosePerPaint;
        }

        // Si el numero de brochas llega a un limite, fusionalas con la textura
        if (brushesInstances >= 20)
        {
            SaveTexture();
        }
    }

    void SelectTextureToPaint()
    {
        int randInd = 0;
        switch (PlayerPrefs.GetInt("Day"))
        {
            case 1:
                // CAMBIAME 1 -> 2
                randInd = Random.Range(0, 1);
                startMaterial.mainTexture = texturesToPaint[randInd];
                break;
        }

        threeStarsTimer += threeStarsTimes[randInd];
        dangerZones += oneStarDangerZones[randInd];
        baseMaterial.mainTexture = mainTex;
    }

    void CheckRedFirstTime()
    {
        Texture2D _tex = startMaterial.mainTexture as Texture2D;
        Color __col = _tex.GetPixel(_tex.width / 2, _tex.height / 2);

        Color[] pix = _tex.GetPixels();

        int redP = 0;
        int whiteP = 0;
        for (int i = 0; i < pix.Length; i += 100)
        {
            if (!(pix[i].r != 1 || pix[i].g != 1 || pix[i].b != 1 || pix[i].a != 1))
            {
                whiteP++;
            }
            if (!(pix[i].r == 1 || pix[i].g != 0 || pix[i].b != 0 || pix[i].a != 1))
            {
                redP++;
            }
        }

        fullRedP = redP / 16.34210526315789f;

        //whiteSlider.value = (float)((whiteP / (pix.Length / 100f)));
        //redSlider.value = (float)((redP / fullRedP));
        gui.peligroPercent = (float)((redP / fullRedP));
    }

    /*** Comprobación del blanco en pantalla ***/
    void Check()
    {
        Texture2D _tex = baseMaterial.mainTexture as Texture2D;
        Color __col = _tex.GetPixel(_tex.width / 2, _tex.height / 2);

        Color[] pix = _tex.GetPixels();

        int whiteP = 0;
        int redP = 0;
        for (int i = 0; i < pix.Length; i+= 100)
        {
            if(!(pix[i].r != 1 || pix[i].g != 1 || pix[i].b != 1 || pix[i].a != 1))
            {
                whiteP++;
            }
            if (!(pix[i].r == 1 || pix[i].g != 0 || pix[i].b != 0 || pix[i].a != 1))
            {
                redP++;
            }
        }

        // Sliders
        whiteSlider.value = (float)((whiteP / (pix.Length / 100f)));
        redSlider.value = (float)((redP / fullRedP));
        gui.peligroPercent = (1f - (float)((redP / fullRedP))) + redPToLoose / 100;
        gui.pinturaPercent = 1f - (pinturaLoosed / 100);
        // Se ha quedado sin pintura
        if(1f - (pinturaLoosed / 100) <= 0f)
        {
            gui.pinturaPercent = 0f;
            GameOver();
        }

        // Ha pintado demsiado rojo
        if (((float)((redP / fullRedP) * 100f) < redPToLoose)){
            GameOver();
        }

        if ((float)(((whiteP + redP) / (pix.Length / 100f)) * 100f) > 97f)
        {
            Win();
        }
    }

    /*** Metodo para comprobar el clickeo sobre el panel ***/
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;
        Vector3 cursorPos = new Vector3(mousePos.x, mousePos.y, 0f);
        Ray cursorRay = Camera.main.ScreenPointToRay(cursorPos);

        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0f);
            RaycastHit mobHit;
            Ray touchRay = Camera.main.ScreenPointToRay(touchPos);

            if(Physics.Raycast(touchRay, out mobHit, 200) && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                MeshCollider meshCollider = mobHit.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                {
                    return false;
                }

                brocha.transform.position = mobHit.point;

                Vector2 pixelUV = new Vector2(mobHit.textureCoord.x, mobHit.textureCoord.y);
                uvWorldPosition.x = pixelUV.x;
                uvWorldPosition.y = pixelUV.y;
                uvWorldPosition.z = 0f;
                return true;
                
            }
        }

        if (Physics.Raycast(cursorRay,out hit, 200) && Input.GetMouseButton(0))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if(meshCollider == null || meshCollider.sharedMesh == null)
            {
                return false;
            }

            brocha.transform.position = hit.point;

            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x;
            uvWorldPosition.y = pixelUV.y;
            uvWorldPosition.z = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    /*** Metodo para fusionar los sprites de las brochas con la textura ***/
    void SaveTexture()
    {
        brushesInstances = 0;
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height),0,0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex;
        foreach (Transform child in brushContainer.transform)
        {
            Destroy(child.gameObject);
        }
        Check();
    }

    /*** Metodo para perder ***/
    void GameOver()
    {
        gameOver = true;
        gui.gameOver = true;
        loosed = true;
    }

    /*** Metodo para ganar ***/
    void Win()
    {
        levelsCompleted++;
        if(levelsCompleted >= 2)
        {
            gui.win = true;
            if(!starsCalculated)
                CalculateStars();
            if (starsCalculated)
                InvokeRepeating("GiveStars", 0, giveStarsTime);
        }
        else
        {
            // Estrellas
            totalDangerZones += 1.0f - redSlider.value;
            gui.next = true;
        }
    }
    
    /*** Calcular estrellas a dar ***/
    void CalculateStars()
    {
        // x Zonas de peligro pintadas = 1 estrella
        if (totalDangerZones < dangerZones)
        {
            totalStars++;
        }
        // Ha completado el juego sin perder = 2 estrellas
        if (!loosed)
        {
            totalStars++;
        }
        // Ha completado el juego en x tiempo = 3 estrellas
        if (totalTimeTimer < threeStarsTimer)
        {
            totalStars++;
        }

        starsCalculated = true;
    }

    /*** Dar estrellas al acabar ***/
    void GiveStars()
    {
        if(totalStars > 0)
        {
            int starIndex = starsGived + 2;
            gui.goToWorldObj.transform.GetChild(starIndex).gameObject.SetActive(true);
            gui.goToWorldObj.transform.GetChild(starIndex).transform.GetChild(0).GetComponent<Animator>().Play("IntroduceStar" + (starIndex - 1));
            totalStars--;
            starsGived++;
        }
        else
        {
            CancelInvoke("GiveStars");
        }
    }

    /*** Volver a intentar ***/
    public void TryAgain()
    {
        ResetStats();
        SelectTextureToPaint();
        CheckRedFirstTime();
    }

    /*** Reiniciar parametros del jugador ***/
    void ResetStats()
    {
        gameOver = false;
        gui.timer = 0;
        gui.startTime = time;
        gui.gameOver = false;
        gui.win = false;
        gui.next = false;
        pinturaLoosed = 0;
        gui.pinturaPercent = 1f - (pinturaLoosed / 100);
    }

    /*** Volver al mundo ***/
    public void GoToWorld()
    {
        if (gui.win && linker != null)
            linker.minigameCompleted = true;
        SceneManager.LoadScene("main");
    }
}

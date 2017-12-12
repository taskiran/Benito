using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HumedadesGameManager : MonoBehaviour {

    public GameObject brush, brushContainer;    // Prefab de la brocha a instanciar y contenedor de los brushed;
    public RenderTexture canvasTexture; // Textura de renderizado que se convierte al material del modelo (plano)
    public Material baseMaterial;   // Material del plano frente a la camara de renderizado de textura
    public Texture2D mainTex;   // Textura del plano frente a la camara de renderizado de textura
    public Material startMaterial;

    public Slider whiteSlider;  // % de pintura blanca
    public Slider redSlider;    // % de zona roja pintada

    private int brushesInstances = 0;   // Controlador de instancias de brochas para fusionar
    private float fullRedP = 0;

    private bool gameOver = false;

	// Use this for initialization
	void Start () {
        // Enlaces
        baseMaterial.mainTexture = mainTex;
        brush.GetComponent<SpriteRenderer>().color = Color.white;
        gameOver = false;

        CheckRedFirstTime();
        
    }
	
	// Update is called once per frame
	void Update () {
        // Variables
        Vector3 uvWorldPosition = Vector3.zero;
        GameObject _brush = null;

        // Si se clicka sobre el plano, instancia una brocha
        if(HitTestUVPosition(ref uvWorldPosition))
        {
            _brush = Instantiate(brush);
            _brush.transform.parent = brushContainer.transform;
            _brush.transform.localPosition = uvWorldPosition;

            brushesInstances++;
        }

        // Si el numero de brochas llega a un limite, fusionalas con la textura
        if (brushesInstances >= 20)
        {
            SaveTexture();
        }

        // BORRAME
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("main");
        }
    }

    void CheckRedFirstTime()
    {
        Texture2D _tex = startMaterial.mainTexture as Texture2D;
        Color __col = _tex.GetPixel(_tex.width / 2, _tex.height / 2);

        Color[] pix = _tex.GetPixels();

        int redP = 0;
        for (int i = 0; i < pix.Length; i += 100)
        {
            if (!(pix[i].r == 1 || pix[i].g != 0 || pix[i].b != 0 || pix[i].a != 1))
            {
                redP++;
            }
        }

        fullRedP = redP / 16.34210526315789f;
    }

    /*** Comprobación del blanco en pantalla ***/
    void Check()
    {
        Texture2D _tex = baseMaterial.mainTexture as Texture2D;
        Color __col = _tex.GetPixel(_tex.width / 2, _tex.height / 2);

        Color[] pix = _tex.GetPixels();

        bool everyWhite = false;
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
        //print((float)((whiteP / (pix.Length / 100f)) * 100f));
        print((float)((redP / fullRedP) * 100f));

        whiteSlider.value = (float)((whiteP / (pix.Length / 100f)));
        redSlider.value = (float)((redP / fullRedP));

        if ((float)((whiteP / (pix.Length / 100f)) * 100f) > 99.5f)
        {
            print("A");
            everyWhite = true;
        }

        // Completamente pintado
        if (everyWhite)
        {
            print("All!");
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
}

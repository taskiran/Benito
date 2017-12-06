using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumedadesGameManager : MonoBehaviour {

    public GameObject brush, brushContainer;
    public RenderTexture canvasTexture;
    public Material baseMaterial;
    public Texture2D mainTex;

    public Slider percent;

    private int brushesInstances = 0;

	// Use this for initialization
	void Start () {
        baseMaterial.mainTexture = mainTex;
        brush.GetComponent<SpriteRenderer>().color = Color.white;

        //Check();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 uvWorldPosition = Vector3.zero;
        GameObject _brush = null;
        if(HitTestUVPosition(ref uvWorldPosition))
        {
            _brush = Instantiate(brush);
            _brush.transform.parent = brushContainer.transform;
            _brush.transform.localPosition = uvWorldPosition;

            brushesInstances++;
        }

        if (brushesInstances >= 20)
        {
            SaveTexture();
        }
    }

    void Check()
    {
        Texture2D _tex = baseMaterial.mainTexture as Texture2D;
        Color __col = _tex.GetPixel(_tex.width / 2, _tex.height / 2);

        Color[] pix = _tex.GetPixels();

        bool everyWhite = true;
        int whiteP = 0;
        for (int i = 0; i < pix.Length; i+= 100)
        {
            if(pix[i].r != 1 || pix[i].g != 1 || pix[i].b != 1 || pix[i].a != 1)
            {
                everyWhite = false;
            }
            else
            {
                whiteP++;
            }
        }

        print((float)((whiteP / (pix.Length / 100f)) * 100f));

        percent.value = (float)((whiteP / (pix.Length / 100f)));

        if (everyWhite)
        {
            print("All!");
        }
    }

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

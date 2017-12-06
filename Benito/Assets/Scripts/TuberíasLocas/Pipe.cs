/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de tuberías
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pipe : MonoBehaviour {
    /* Propiedades publicas */
    public Sprite[] LSprites;
    public Sprite TSprite, startSprite, endSprite;
    public Sprite[] ISprites;
    public Sprite[] LDropsSprites;
    public Sprite TDropSprite, startDropSprite, endDropSprite;
    public Sprite[] IDropsSprites;

    [HideInInspector]
    public int type = 0, x, y, preType = 0;
    [HideInInspector]
    public bool startPipe = false, endPipe = false, prePipe = false;

    /* Propiedades privadas */
    private SpriteRenderer sRenderer;
    private int ind = 0;
    private Generator generator;
    private GameManager gameManager;
    private GameObject waterParent;
    private float scale, _scale, limitScale;
    private float waterSpeed;
    private bool connectedDown, connectedUp, connectedRight, connectedLeft;
    private bool entersDown, entersUp, entersRight, entersLeft;
    private bool fillingPipe = false;
    private GameObject waterDropsObject;

    /*** AWAKE ***/
    private void Awake()
    {
        // Enlaces
        waterParent = transform.GetChild(0).gameObject;
        sRenderer = GetComponent<SpriteRenderer>();
        generator = GameObject.Find("GameManager").GetComponent<Generator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterSpeed = gameManager.waterSpeed;
        waterDropsObject = transform.GetChild(0).transform.GetChild(1).transform.gameObject;
    }

    /*** START ***/
    void Start () {
        
        // Inicializacion de las tuberias
        scale = transform.localScale.x;
        // Tuberia especial
        if (startPipe)
        {
            sRenderer.sprite = startSprite;
            waterDropsObject.GetComponent<SpriteRenderer>().sprite = startDropSprite;
            transform.GetComponent<SpriteMask>().sprite = startSprite;
            
            if (y > 0)
            {
                transform.localScale = new Vector3(scale, -scale, scale);
            }

            if (x == 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else if (x == generator.rows - 1)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            waterParent.transform.localPosition = new Vector3(0, -1, 0);
            _scale = 0;
            type = 4;
            return;
        }
        else if (endPipe)
        {
            sRenderer.sprite = endSprite;
            transform.GetComponent<SpriteMask>().sprite = endSprite;
            waterDropsObject.GetComponent<SpriteRenderer>().sprite = endDropSprite;

            waterParent.transform.localPosition = new Vector3(0, -1, 0);
            if (y > 0)
            {
                transform.localScale = new Vector3(scale, -scale, scale);
            }

            if (x == 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else if (x == generator.rows - 1)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            type = 5;
            return;
        }

        waterParent.transform.localScale = new Vector3(1, 0, 1);
        switch (type)
        {
            case (0):   // L
                waterParent.transform.localPosition = new Vector3(0, -1, 0);
                break;
            case (1):   // T
                waterParent.transform.localPosition = new Vector3(0, -1, 0);
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        UpdatePipe();
	}

    /*** Actualizador de tuberias ***/
    void UpdatePipe()
    {
        waterSpeed = gameManager.waterSpeed;
        // Segun el tipo, controla la tuberia
        switch (type)
        {
            case (0):   // L
                sRenderer.sprite = LSprites[ind];
                transform.GetComponent<SpriteMask>().sprite = LSprites[ind];
                waterDropsObject.GetComponent<SpriteRenderer>().sprite = LDropsSprites[ind];

                if (ind == 0)   // D-R
                {
                    connectedDown = true;
                    connectedRight = true;
                    connectedLeft = false;
                    connectedUp = false;

                    if (fillingPipe)
                    {
                        if (entersDown)
                        {
                            waterParent.transform.localPosition = new Vector3(0, -1f, 0);
                            limitScale = 1.3f;
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localPosition = new Vector3(1, 0, 0);
                            limitScale = 1.3f;
                        }
                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersDown)
                            {
                                if (generator.pipes[x + 1, y].GetComponent<Pipe>().connectedLeft)
                                {
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().entersLeft = true;
                                }
                                else if (generator.pipes[x + 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersRight)
                            {
                                if (generator.pipes[x, y - 1].GetComponent<Pipe>().connectedUp)
                                {
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().entersUp = true;
                                }
                                else if (generator.pipes[x, y - 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersDown)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }
                        
                    }
                }
                else if (ind == 1)  // U-R
                {
                    connectedDown = false;
                    connectedRight = true;
                    connectedLeft = false;
                    connectedUp = true;

                    if (fillingPipe)
                    {

                        if (entersUp)
                        {
                            waterParent.transform.localPosition = new Vector3(0, 1f, 0);
                            limitScale = 1.3f;
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localPosition = new Vector3(1, 0, 0);
                            limitScale = 1.3f;
                        }


                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersUp)
                            {
                                if (generator.pipes[x + 1, y].GetComponent<Pipe>().connectedLeft)
                                {
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().entersLeft = true;
                                }
                                else if (generator.pipes[x + 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersRight)
                            {
                                if (generator.pipes[x, y + 1].GetComponent<Pipe>().connectedUp)
                                {
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().entersDown = true;
                                }
                                else if (generator.pipes[x, y + 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersUp)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }

                    }
                }
                else if (ind == 2)  // U-L
                {
                    connectedDown = false;
                    connectedRight = true;
                    connectedLeft = true;
                    connectedUp = false;

                    if (fillingPipe)
                    {

                        if (entersLeft)
                        {
                            waterParent.transform.localPosition = new Vector3(-1, 0, 0);
                            limitScale = 1.3f;
                        }
                        else if (entersUp)
                        {
                            waterParent.transform.localPosition = new Vector3(0, 1, 0);
                            limitScale = 1.3f;
                        }


                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersLeft)
                            {
                                if (generator.pipes[x, y + 1].GetComponent<Pipe>().connectedDown)
                                {
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().entersDown = true;
                                }
                                else if (generator.pipes[x, y + 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersUp)
                            {
                                if (generator.pipes[x - 1, y].GetComponent<Pipe>().connectedRight)
                                {
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().entersRight = true;
                                }
                                else if (generator.pipes[x - 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersLeft)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }
                        else if (entersUp)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }
                        
                    }
                }
                else if (ind == 3)  // L-D
                {
                    connectedDown = true;
                    connectedRight = false;
                    connectedLeft = true;
                    connectedUp = false;

                    if (fillingPipe)
                    {

                        if (entersLeft)
                        {
                            waterParent.transform.localPosition = new Vector3(-1, 0, 0);
                            limitScale = 1.3f;
                        }
                        else if (entersDown)
                        {
                            waterParent.transform.localPosition = new Vector3(0, -1f, 0);
                            limitScale = 1.3f;
                        }


                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersLeft)
                            {
                                if (generator.pipes[x, y - 1].GetComponent<Pipe>().connectedUp)
                                {
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().entersUp = true;
                                }
                                else if (generator.pipes[x, y - 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersDown)
                            {
                                if (generator.pipes[x - 1, y].GetComponent<Pipe>().connectedRight)
                                {
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().entersRight = true;
                                }
                                else if (generator.pipes[x - 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersLeft)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }
                        else if (entersDown)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }

                    }
                }
                break;
            case (1):   // T
                sRenderer.sprite = TSprite;
                transform.GetComponent<SpriteMask>().sprite = TSprite;
                waterDropsObject.GetComponent<SpriteRenderer>().sprite = TDropSprite;

                connectedDown = true;
                connectedRight = true;
                connectedLeft = true;
                connectedUp = true;

                if (fillingPipe)
                {
                    if (entersLeft)
                    {
                        waterParent.transform.localPosition = new Vector3(-1f, 0, 0);
                        limitScale = 2f;
                    }
                    else if (entersRight)
                    {
                        waterParent.transform.localPosition = new Vector3(1, 0, 0);
                        limitScale = 2f;
                    }
                    else if (entersUp)
                    {
                        waterParent.transform.localPosition = new Vector3(0, 1f, 0);
                        waterParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180f));
                        limitScale = 2f;
                    }
                    else if (entersDown)
                    {
                        waterParent.transform.localPosition = new Vector3(0, -1f, 0);
                        limitScale = 2f;
                    }


                    _scale += waterSpeed * Time.deltaTime;
                    if (_scale >= 1f)
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersLeft)
                            {
                                if (generator.pipes[x + 1, y].GetComponent<Pipe>().connectedLeft)
                                {
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().entersLeft = true;
                                }
                                else if (generator.pipes[x + 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersRight)
                            {
                                if (generator.pipes[x - 1, y].GetComponent<Pipe>().connectedRight)
                                {
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().entersRight = true;
                                }
                                else if (generator.pipes[x - 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersUp)
                            {
                                if (generator.pipes[x, y - 1].GetComponent<Pipe>().connectedUp)
                                {
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().entersUp = true;
                                }
                                else if (generator.pipes[x, y - 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersDown)
                            {
                                if (generator.pipes[x, y + 1].GetComponent<Pipe>().connectedDown)
                                {
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().entersDown = true;
                                }
                                else if (generator.pipes[x, y + 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                    if (entersLeft)
                    {
                        waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                    }
                    else if (entersRight)
                    {
                        waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                    }
                    else if (entersUp)
                    {
                        waterParent.transform.localScale = new Vector3(1, _scale, 1);
                    }
                    else if (entersDown)
                    {
                        waterParent.transform.localScale = new Vector3(1, _scale, 1);
                    }

                }
                break;
            case (2):   // I
                sRenderer.sprite = ISprites[ind];
                transform.GetComponent<SpriteMask>().sprite = ISprites[ind];
                waterDropsObject.GetComponent<SpriteRenderer>().sprite = IDropsSprites[ind];

                if (ind == 0)   // L-R
                {
                    connectedDown = false;
                    connectedRight = true;
                    connectedLeft = true;
                    connectedUp = false;

                    if (fillingPipe)
                    {

                        if (entersLeft)
                        {
                            waterParent.transform.localPosition = new Vector3(-1f, 0, 0);
                            limitScale = 2f;
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localPosition = new Vector3(1f, 0, 0);
                            limitScale = 2f;
                        }


                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersLeft)
                            {
                                if (generator.pipes[x + 1, y].GetComponent<Pipe>().connectedLeft)
                                {
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x + 1, y].GetComponent<Pipe>().entersLeft = true;
                                }
                                else if (generator.pipes[x + 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersRight)
                            {
                                if (generator.pipes[x - 1, y].GetComponent<Pipe>().connectedRight)
                                {
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x - 1, y].GetComponent<Pipe>().entersRight = true;
                                }
                                else if (generator.pipes[x - 1, y].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersLeft)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }
                        else if (entersRight)
                        {
                            waterParent.transform.localScale = new Vector3(_scale, 1, 1);
                        }

                    }
                }
                else if (ind == 1)  // U-D
                {
                    connectedDown = true;
                    connectedRight = false;
                    connectedLeft = false;
                    connectedUp = true;

                    waterParent.transform.localPosition = new Vector3(0, -1, 0);

                    if (fillingPipe)
                    {
                        if (entersDown)
                        {
                            waterParent.transform.localPosition = new Vector3(0, -1f, 0);
                            limitScale = 2f;
                        }
                        else if (entersUp)
                        {
                            waterParent.transform.localPosition = new Vector3(0, 1f, 0);
                            limitScale = 1.7f;
                        }


                        _scale += waterSpeed * Time.deltaTime;
                        if (_scale >= limitScale)
                        {
                            _scale = limitScale;

                            if (entersDown)
                            {
                                if (generator.pipes[x, y + 1].GetComponent<Pipe>().connectedDown)
                                {
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y + 1].GetComponent<Pipe>().entersDown = true;
                                }
                                else if (generator.pipes[x, y + 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                            else if (entersUp)
                            {
                                if (generator.pipes[x, y - 1].GetComponent<Pipe>().connectedUp)
                                {
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().fillingPipe = true;
                                    generator.pipes[x, y - 1].GetComponent<Pipe>().entersUp = true;
                                }
                                else if (generator.pipes[x, y - 1].GetComponent<Pipe>().type == 5)
                                {
                                    Win();
                                }
                                else
                                {
                                    GameOver();
                                }
                            }
                        }

                        if (entersDown)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }
                        else if (entersUp)
                        {
                            waterParent.transform.localScale = new Vector3(1, _scale, 1);
                        }

                    }
                }
                break;
            case (4):   // Start
                waterParent.transform.localPosition = new Vector3(0, -1, 0);
                limitScale = 1.7f;

                _scale += waterSpeed * Time.deltaTime;
                if (_scale >= limitScale)
                {
                    _scale = limitScale;

                    if (y == 0)
                    {
                        if(generator.pipes[x, y + 1].GetComponent<Pipe>().connectedDown)
                        {
                            generator.pipes[x, y + 1].GetComponent<Pipe>().entersDown = true;
                            generator.pipes[x, y + 1].GetComponent<Pipe>().fillingPipe = true;
                        }
                        else
                        {
                            GameOver();
                        }
                    }
                    else if (y == 8)
                    {
                        if (generator.pipes[x, y - 1].GetComponent<Pipe>().connectedUp)
                        {
                            generator.pipes[x, y - 1].GetComponent<Pipe>().entersUp = true;
                            generator.pipes[x, y - 1].GetComponent<Pipe>().fillingPipe = true;
                        }
                        else
                        {
                            GameOver();
                        }
                    }
                }
                waterParent.transform.localScale = new Vector3(1, _scale, 1);
                break;

            case (5):   // End
                
                break;
        }
    }

    public void ChangePipe()
    {
        if (!fillingPipe)
        {
            switch (type)
            {
                case (0):
                    ind++;
                    if (ind >= LSprites.Length)
                        ind = 0;
                    break;
                case (2):
                    ind++;
                    if (ind >= ISprites.Length)
                        ind = 0;
                    break;
            }
        }
    }

    public void ChangePipeType()
    {
        if (!fillingPipe)
        {
            type++;
            if(type > 2)
            {
                type = 0;
            }
        }
    }

    private void Win()
    {
        gameManager.win = true;
    }

    private void GameOver()
    {
        gameManager.gameOver = true;
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de tuberías
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

/*---------------------------------------------------------------------------*
 * Benito: El centinela de la escuela
 * Generador del escenario de tuberías
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------*
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Generator : MonoBehaviour {

    /* Propiedades publicas */
    public GameObject pipePf;
    public Sprite dumSprite;
    public int[] xPreType, yPreType, preType;
    public GameObject[,] pipes;
    [HideInInspector]
    public int rows, level, levelsCompleted;

    /* Propiedades privadas */
    private int __x = 0;
    private bool chked = false;

    /*** Tipos de tuberías ***/
    /* 0->  Tipo cruz o T
     * 1->  Tipo L
     * 2->  Recto
     * 3->  Entrada
     * 4->  Salida
     * Tamaño REAL del escenario:
     * X->  De 2 a 12
     * Y->  De 0 a 8
     */
    [HideInInspector]
    public int[,,] stage = {{/*0  1      2   3   4   5   6   7   8   9   10  11  12     13  14*/       // <-- Nivel 1
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  4,     0,  0 },   //0
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //1
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //2
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //3
                             {0, 0,     0,  0,  0,  0,  0,  0,  1,  2,  2,  2,  1,     0,  0 },   //4
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //5
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //6
                             {0, 0,     1,  2,  2,  2,  2,  2,  1,  0,  0,  0,  0,     0,  0 },   //7
                             {0, 0,     3,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //8 

                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //9 
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //10
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //11
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //12
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //13
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 }    //14
                            },
                             {/*0  1      2   3   4   5   6   7   8   9   10  11  12     13  14*/       // <-- Nivel 2
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  4,     0,  0 },   //0
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //1
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //2
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //3
                             {0, 0,     0,  0,  0,  0,  0,  0,  1,  2,  2,  2,  1,     0,  0 },   //4
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //5
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //6
                             {0, 0,     0,  0,  0,  0,  1,  2,  1,  0,  0,  0,  0,     0,  0 },   //7
                             {0, 0,     0,  0,  0,  0,  3,  0,  0,  0,  0,  0,  0,     0,  0 },   //8

                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //9 
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //10
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //11
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //12
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //13
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 }    //14
                             },
                            {/*0  1      2   3   4   5   6   7   8   9   10  11  12     13  14*/       // <-- Nivel 3
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  4,     0,  0 },   //0
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //1
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //2
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //3
                             {0, 0,     0,  0,  0,  0,  0,  0,  1,  2,  2,  2,  1,     0,  0 },   //4
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //5
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //6
                             {0, 0,     1,  2,  2,  2,  2,  2,  1,  0,  0,  0,  0,     0,  0 },   //7
                             {0, 0,     3,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //8 

                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //9 
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //10
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //11
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //12
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //13
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 }    //14
                            },
                             {/*0  1      2   3   4   5   6   7   8   9   10  11  12     13  14*/       // <-- Nivel 4
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  4,     0,  0 },   //0
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //1
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //2
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,     0,  0 },   //3
                             {0, 0,     0,  0,  0,  0,  0,  0,  1,  2,  2,  2,  1,     0,  0 },   //4
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //5
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //6
                             {0, 0,     0,  0,  0,  0,  1,  2,  1,  0,  0,  0,  0,     0,  0 },   //7
                             {0, 0,     0,  0,  0,  0,  3,  0,  0,  0,  0,  0,  0,     0,  0 },   //8

                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //9 
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //10
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //11
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //12
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 },   //13
                             {0, 0,     0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,     0,  0 }    //14
                             }};

    // Use this for initialization
    void Start () {
        //PlayerPrefs.SetInt("Day", 2);
        levelsCompleted = 0;
        rows = 15;
        pipes = new GameObject[rows, rows];
        SetRandomlevel();
        GenerateStage();
	}

    public void DestroyStage()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Destroy(pipes[x, y]);
            }
        }

        pipes = new GameObject[rows, rows];
    }

    public void GenerateStage()
    {
        // Calcula la escala
        // Para 10 columnas ---> escala 1
        // Para rows columnas ---> x
        float _scale = 1f * 10f / rows;
        // Variables para las operaciones siguientes
        float lastX = 0, lastY = 0, _y = 0, _x = 0;
        GameObject _pipe = null;

        // Bucle de creacion de tuberias
        for (int x = 1; x <= rows; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Crea la tuberia
                GameObject pipe = Instantiate(pipePf);
                pipe.name = "Pipe at: " + (x - 1)+ ", " + y;
                // Dale un sprite
                pipe.GetComponent<SpriteRenderer>().sprite = dumSprite;
                // Calcula la posicion
                if(y == 0)
                {
                    _y = _scale;
                }
                else
                {
                    _y = lastY + _scale * 2;
                }

                if(x == 1)
                {
                    _x = _scale;
                }
                else
                {
                    _x = lastX + _scale * 2;
                }
                // Aplica la posicion
                pipe.transform.position = new Vector3(_x, _y, 0);
                // Aplica la escala
                pipe.transform.localScale = new Vector3(_scale, _scale, _scale);

                pipe.GetComponent<Pipe>().x = x - 1;
                pipe.GetComponent<Pipe>().y = y;

                /*** Tipos de tuberías ***/
                /* 0->  Tipo cruz o T
                 * 1->  Tipo L
                 * 2->  Recto
                 * 3->  Entrada
                 * 4->  Salida
                 */
                int type = stage[level, y, x - 1];
                switch (type)
                {
                    case (0):   // Cruz o T
                        pipe.GetComponent<Pipe>().type = 1;
                        break;
                    case (1):   // Tipo L
                        pipe.GetComponent<Pipe>().type = 0;
                        break;
                    case (2):   // Recto
                        pipe.GetComponent<Pipe>().type = 2;
                        break;
                    case (3):   // Entrada
                        pipe.GetComponent<Pipe>().startPipe = true;
                        break;
                    case (4):   // Salida
                        pipe.GetComponent<Pipe>().endPipe = true;
                        break;
                }

                _pipe = pipe;

                lastY = pipe.transform.position.y;

                pipes[x - 1, y] = pipe;
            }
            // Reinicia las variables al cambiar de columna
            lastX = _pipe.transform.position.x;
            lastY = 0;
        }
    }

    public void SetRandomlevel()
    {
        switch (PlayerPrefs.GetInt("Day"))
        {
            case (1):
                level = Random.Range(0, 2);
                break;
            case (2):
                level = Random.Range(2, 4);
                break;
            case (3):
                level = Random.Range(4, 6);
                break;
        }
    }
}


/*---------------------------------------------------------------------------*
 * Benito: El centinela de la escuela
 * Generador del escenario de tuberías
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------*
 */

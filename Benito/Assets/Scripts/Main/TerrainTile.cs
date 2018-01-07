/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los tiles
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour {

    public float speed = 20f;

    [HideInInspector]
    public int x, y, z;

    [HideInInspector]
    public bool isUpstairsTile;

    private GameGenerator generator;
    private Player player;

    private bool positionated = false;

    private void Awake()
    {
        generator = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<GameGenerator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (!positionated)
            PositionateTile();
    }

    void PositionateTile()
    {
        // Si este tile está en el piso de abajo...
        if (!isUpstairsTile)
        {
            if (transform.position.y > -0.5f)
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
                positionated = true;
            }
        }
        else
        {
            if (transform.position.y > 10.4f)
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 10.4f, transform.position.z);
                positionated = true;
            }
        }
        
    }
        
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Controlador de los tiles
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

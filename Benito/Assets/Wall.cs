/*---------------------------------------------------------------------------*
 * Benito: El centinela de la escuela
 * Controlador de paredes
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------*
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    /* Propiedades públicas */
    public float maxTransparencyDistance = 6.5f;
    public float minTransparencyDistance = 2f;

    /* Propiedades privadas */
    private GameObject player;
    private Vector3 playerPos;
    private float distance;
    private float scaledDistance;

    private Renderer rend;

    /*** AWAKE ***/
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponent<Renderer>();
    }
	
	/*** UPDATE ***/
	void Update () {
        playerPos = player.transform.position;  // Almacena la posicion del player
        distance = Vector3.Distance(transform.position, playerPos); // Calcula la distancia entre la pared y el player
        scaledDistance = (distance - minTransparencyDistance) / (maxTransparencyDistance - minTransparencyDistance);   // Normaliza la distancia
        scaledDistance = Mathf.Clamp01(scaledDistance); // Limita la distancia entre 0 - 1

        // Aplica el color con la opacidad calculada previamente
        Color col = rend.material.color;
        col.a = scaledDistance;
        rend.material.color = col;
	}
}


/*---------------------------------------------------------------------------*
 * Benito: El centinela de la escuela
 * Controlador de paredes
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------*
 */

﻿/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Generador del escenario principal
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour {

    public int numberOfZTiles, numberOfXTiles;
    public GameObject tilePrefab, terrainTilesParent;
    public float spaceBetweenTiles, heightSpawn = 3f, spawnSpeed = .0005f;
    public TerrainTile[,] terrainTiles;

    // Use this for initialization
    void Awake () {
        terrainTiles = new TerrainTile[numberOfXTiles, numberOfZTiles];
        StartCoroutine(GenerateTerrainTiles());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator GenerateTerrainTiles()
    {
        for (int z = 0; z > -numberOfZTiles; z--)
        {
            for (int x = 0; x > -numberOfXTiles; x--)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), heightSpawn, z + (spaceBetweenTiles * z)), Quaternion.identity);
                tile.transform.parent = terrainTilesParent.transform;
                tile.GetComponent<TerrainTile>().z = z * -1;
                tile.GetComponent<TerrainTile>().x = x * -1;

                terrainTiles[x * -1, z * -1] = tile.GetComponent<TerrainTile>();
            }
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Generador del escenario principal
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */
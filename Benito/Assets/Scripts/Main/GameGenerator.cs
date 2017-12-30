/*---------------------------------------------------------------------------
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
    public GameObject tilePrefab, firstFloorTerrainTilesParent, secondFloorTerrainTilesParent;
    public float spaceBetweenTiles, heightSpawn = 3f, spawnSpeed = .0005f;
    public TerrainTile[,,] terrainTiles;

    // Use this for initialization
    void Awake () {
        terrainTiles = new TerrainTile[numberOfXTiles, 2, numberOfZTiles];
        StartCoroutine(GenerateTerrainTiles());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator GenerateTerrainTiles()
    {
        // Genera el piso de abajo
        for (int z = 0; z > -numberOfZTiles; z--)
        {
            for (int x = 0; x > -numberOfXTiles; x--)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), heightSpawn, z + (spaceBetweenTiles * z)), Quaternion.identity);
                tile.transform.parent = firstFloorTerrainTilesParent.transform;
                tile.GetComponent<TerrainTile>().z = z * -1;
                tile.GetComponent<TerrainTile>().x = x * -1;

                terrainTiles[x * -1, 0 ,z * -1] = tile.GetComponent<TerrainTile>();
            }
            yield return new WaitForSeconds(spawnSpeed);
        }

        // Genera el piso de arriba
        for (int z = 0; z > -numberOfZTiles; z--)
        {
            for (int x = 0; x > -numberOfXTiles; x--)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), 10f + heightSpawn, z + (spaceBetweenTiles * z)), Quaternion.identity);
                tile.transform.parent = secondFloorTerrainTilesParent.transform;
                tile.GetComponent<TerrainTile>().z = z * -1;
                tile.GetComponent<TerrainTile>().x = x * -1;
                tile.GetComponent<TerrainTile>().isUpstairsTile = true;

                terrainTiles[x * -1, 1 ,z * -1] = tile.GetComponent<TerrainTile>();
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

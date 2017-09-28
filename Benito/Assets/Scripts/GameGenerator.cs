using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour {

    public int numberOfZTiles, numberOfXTiles;
    public GameObject tilePrefab, terrainTilesParent;
    public float heightSpawn = 3f;

    public TerrainTile[,] terrainTiles;

	// Use this for initialization
	void Start () {
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
                GameObject tile = Instantiate(tilePrefab, new Vector3(z, heightSpawn, x), Quaternion.identity);
                tile.transform.parent = terrainTilesParent.transform;
                tile.GetComponent<TerrainTile>().z = z;
                tile.GetComponent<TerrainTile>().x = x;

                terrainTiles[Mathf.Abs(x), Mathf.Abs(z)] = tile.GetComponent<TerrainTile>();
                yield return new WaitForSeconds(.0003f);
            }
        }
    }
}

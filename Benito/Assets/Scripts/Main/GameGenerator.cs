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
    public GameObject tilePrefab, firstFloorTerrainTilesParent, secondFloorTerrainTilesParent, suelo1, escenario1;
    public float spaceBetweenTiles, heightSpawn = 3f, spawnSpeed = .0005f;
    public TerrainTile[,,] terrainTiles;
    [HideInInspector]
    public bool terrainGenerated, floorGenerated, sceneGenerated, dayStarted, firstFloor, secondFloor;

    private Color sceneColor;
    private float alpha;

    // Use this for initialization
    void Awake () {
        terrainTiles = new TerrainTile[numberOfXTiles, 2, numberOfZTiles];
        
        terrainGenerated = false;
        alpha = 0f;
        dayStarted = false;
        sceneGenerated = false;

        suelo1.SetActive(false);
        escenario1.SetActive(false);

        firstFloorTerrainTilesParent.SetActive(true);
        secondFloorTerrainTilesParent.SetActive(false);
    }

    private void Start()
    {
        if(!dayStarted)
            StartCoroutine(GenerateTerrainTiles());
        else
        {
            // Genera el piso de abajo
            for (int z = 0; z > -numberOfZTiles; z--)
            {
                for (int x = 0; x > -numberOfXTiles; x--)
                {
                    GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), -0.5f, z + (spaceBetweenTiles * z)), Quaternion.identity);
                    tile.transform.parent = firstFloorTerrainTilesParent.transform;
                    tile.GetComponent<TerrainTile>().z = z * -1;
                    tile.GetComponent<TerrainTile>().x = x * -1;

                    terrainTiles[x * -1, 0, z * -1] = tile.GetComponent<TerrainTile>();
                }
            }

            // Genera el piso de arriba
            for (int z = 0; z > -numberOfZTiles; z--)
            {
                for (int x = 0; x > -numberOfXTiles; x--)
                {
                    GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), 10.4f, z + (spaceBetweenTiles * z)), Quaternion.identity);
                    tile.transform.parent = secondFloorTerrainTilesParent.transform;
                    tile.GetComponent<TerrainTile>().z = z * -1;
                    tile.GetComponent<TerrainTile>().x = x * -1;
                    tile.GetComponent<TerrainTile>().isUpstairsTile = true;

                    terrainTiles[x * -1, 1, z * -1] = tile.GetComponent<TerrainTile>();
                }
            }

            terrainGenerated = true;

            // Suelo
            suelo1.SetActive(true);
            alpha = 1;

            sceneColor = suelo1.GetComponent<Renderer>().material.color;
            sceneColor.a = alpha;
            suelo1.GetComponent<Renderer>().material.color = sceneColor;

            floorGenerated = true;

            // Escenario
            escenario1.SetActive(true);
            alpha = 1;
            for (int i = 0; i < escenario1.transform.GetChild(0).childCount; i++)
            {
                sceneColor = escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color;
                sceneColor.a = alpha;
                escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color = sceneColor;
            }

            sceneGenerated = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (!dayStarted)
        {
            if (terrainGenerated )
            {
                if (!floorGenerated) GenerateFloor();
                else if (!sceneGenerated) GenerateScene();
            }
        }
	}

    void GenerateFloor()
    {
        suelo1.SetActive(true);
        if(alpha < 1)
        {
            alpha += Time.deltaTime;

            sceneColor = suelo1.GetComponent<Renderer>().material.color;
            sceneColor.a = alpha;
            suelo1.GetComponent<Renderer>().material.color = sceneColor;
        }
        else
        {
            alpha = 1;

            sceneColor = suelo1.GetComponent<Renderer>().material.color;
            suelo1.GetComponent<Renderer>().material.color = sceneColor;

            alpha = 0;

            for (int i = 0; i < escenario1.transform.GetChild(0).childCount; i++)
            {
                sceneColor = escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color;
                sceneColor.a = alpha;
                escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color = sceneColor;
            }

            floorGenerated = true;
        }
    }

    void GenerateScene()
    {
        escenario1.SetActive(true);
        if (alpha < 1)
        {
            alpha += Time.deltaTime;
            for (int i = 0; i < escenario1.transform.GetChild(0).childCount; i++)
            {
                sceneColor = escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color;
                sceneColor.a = alpha;
                escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color = sceneColor;
            }
        }
        else
        {
            alpha = 1;
            for (int i = 0; i < escenario1.transform.GetChild(0).childCount; i++)
            {
                sceneColor = escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color;
                sceneColor.a = alpha;
                escenario1.transform.GetChild(0).GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color = sceneColor;
            }

            firstFloorTerrainTilesParent.SetActive(true);
            secondFloorTerrainTilesParent.SetActive(false);

            sceneGenerated = true;
        }
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
                GameObject tile = Instantiate(tilePrefab, new Vector3(x + (spaceBetweenTiles * x), 10.4f, z + (spaceBetweenTiles * z)), Quaternion.identity);
                tile.transform.parent = secondFloorTerrainTilesParent.transform;
                tile.GetComponent<TerrainTile>().z = z * -1;
                tile.GetComponent<TerrainTile>().x = x * -1;
                tile.GetComponent<TerrainTile>().isUpstairsTile = true;

                terrainTiles[x * -1, 1 ,z * -1] = tile.GetComponent<TerrainTile>();
            }
            yield return new WaitForSeconds(spawnSpeed);
        }

        terrainGenerated = true;
    }
}


/*---------------------------------------------------------------------------
 * Benito: El centinela de la escuela
 * Generador del escenario principal
 * © David Basagaña Mimoso
 *---------------------------------------------------------------------------
 */

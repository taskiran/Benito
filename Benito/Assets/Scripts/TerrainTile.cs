using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour {

    public int x, y, z, steps;
    public bool completeObstacle, pXObstacle, mXObstacle, pZObstacle, mZObstacle;

    public bool target, search, hasSearched, reverse, backwarded;
    public List<Vector3> chainPositions = new List<Vector3>();

    private bool positionated;
    private GameGenerator generator;
    private Player player;

    private void Awake()
    {
        generator = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameGenerator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Start()
    {
        // Borrame
        int i = Random.Range(0,9);
        print(i);
    }

    private void Update()
    {
        if (!positionated)
            PositionateTile();

        if (!player.pathFinded)
        {
            if (search && !generator.reverseSearch && !completeObstacle)
                AddPositionToNeighbours();

            if (reverse)
            {
                generator.reverseSearch = true;
                GoBackwards();
            }
        }
        

        // Borrame
        if (!backwarded)
        {
            Color col = Color.red;
            //col.r = steps * .01f;
            //print(0.1f * steps);
            //GetComponent<Renderer>().material.color = col;
        }
    }

    void PositionateTile()
    {
        if (transform.position.y > 0)
        {
            transform.Translate(Vector3.down * 40 * Time.deltaTime);
        }
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    void GoBackwards()
    {
        // Si este ya es el paso 1...
        if(steps == 1)
        {
            chainPositions.Add(transform.position);
            player.i = chainPositions.Count - 1;
            player.positionsToTranslate = chainPositions;
            player.pathFinded = true;
            

            // Borrame
            backwarded = true;
            //GetComponent<Renderer>().material.color = Color.yellow;
        }
        // Si aun queda recorrido
        else
        {
            // Busca al tile vecino con el paso anterior a este
            if (x > 0)
            {
                if (generator.terrainTiles[x - 1, z].steps == steps -1 && !backwarded && !generator.terrainTiles[x - 1, z].search)
                {
                    // Almacena las posiciones guardadas
                    chainPositions.Add(transform.position);
                    generator.terrainTiles[x - 1, z].chainPositions = chainPositions;
                    // Indica que vaya para atrás
                    generator.terrainTiles[x - 1, z].search = false;
                    generator.terrainTiles[x - 1, z].reverse = true;
                    // Indica a este tile que no busque mas vecinos
                    backwarded = true;

                    //Borrame
                    //generator.terrainTiles[x - 1, z].transform.GetComponent<Renderer>().material.color = Color.white;
                }
            }
            if (x < generator.numberOfXTiles - 1)
            {
                if (generator.terrainTiles[x + 1, z].steps == steps - 1 && !backwarded && !generator.terrainTiles[x + 1, z].search)
                {
                    // Almacena las posiciones guardadas
                    chainPositions.Add(transform.position);
                    generator.terrainTiles[x + 1, z].chainPositions = chainPositions;
                    // Indica que vaya para atrás
                    generator.terrainTiles[x + 1, z].search = false;
                    generator.terrainTiles[x + 1, z].reverse = true;
                    // Indica a este tile que no busque mas vecinos
                    backwarded = true;

                    //Borrame
                    //generator.terrainTiles[x + 1, z].transform.GetComponent<Renderer>().material.color = Color.white;
                }
            }
            if (z > 0)
            {
                if (generator.terrainTiles[x, z - 1].steps == steps - 1 && !backwarded && !generator.terrainTiles[x, z - 1].search)
                {
                    // Almacena las posiciones guardadas
                    chainPositions.Add(transform.position);
                    generator.terrainTiles[x, z - 1].chainPositions = chainPositions;
                    // Indica que vaya para atrás
                    generator.terrainTiles[x, z - 1].search = false;
                    generator.terrainTiles[x, z - 1].reverse = true;
                    // Indica a este tile que no busque mas vecinos
                    backwarded = true;

                    //Borrame
                   // generator.terrainTiles[x , z - 1].transform.GetComponent<Renderer>().material.color = Color.white;
                }
            }

            if (z < generator.numberOfZTiles - 1)
            {
                if (generator.terrainTiles[x, z + 1].steps == steps - 1 && !backwarded && !generator.terrainTiles[x, z + 1].search)
                {
                    // Almacena las posiciones guardadas
                    chainPositions.Add(transform.position);
                    generator.terrainTiles[x, z + 1].chainPositions = chainPositions;
                    // Indica que vaya para atrás
                    generator.terrainTiles[x, z + 1].search = false;
                    generator.terrainTiles[x, z + 1].reverse = true;
                    // Indica a este tile que no busque mas vecinos
                    backwarded = true;

                    //Borrame
                    //generator.terrainTiles[x, z + 1].transform.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
    }

    void AddPositionToNeighbours()
    {
        // Si aun no se encontrado al objetivo...
        if (!player.pathFinded)
        {
            // +X
            if (!mZObstacle)
            {
                // Si es = 0
                if (x == 0 && z == 0)
                {
                    if (generator.terrainTiles[x + 1, z].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        //GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x + 1, z].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x + 1, z].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x + 1, z].search = true;
                    }
                }

                // Si la X es menor al total de X Tiles
                if (x < generator.numberOfXTiles - 1)
                {
                    if (generator.terrainTiles[x + 1, z].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        //GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x + 1, z].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x + 1, z].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x + 1, z].search = true;
                    }
                }
            }

            // -X
            if (!pZObstacle)
            {
                // Si la X es mayor a 0
                if (x > 0)
                {
                    if (generator.terrainTiles[x - 1, z].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        //GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x - 1, z].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x - 1, z].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x - 1, z].search = true;
                    }
                }

                
            }

            // +Z
            if (!mXObstacle)
            {
                // Si es = 0
                if (x == 0 && z == 0)
                {
                    if (generator.terrainTiles[x, z + 1].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x, z + 1].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x, z + 1].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x, z + 1].search = true;
                    }
                }

                // Si la Z es menor al total de Z Tiles
                if (z < generator.numberOfZTiles - 1)
                {
                    if (generator.terrainTiles[x, z + 1].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        //GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x, z + 1].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x, z + 1].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x, z + 1].search = true;
                    }
                }
            }

            // -Z
            if (!pXObstacle)
            {
                // Si la Z es mayor a 0
                if (z > 0)
                {
                    if (generator.terrainTiles[x, z - 1].target)
                    {
                        // Indica a este tile a ir para atrás
                        chainPositions.Add(transform.position);
                        reverse = true;
                        search = false;

                        //Borrame
                        //GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (!generator.terrainTiles[x, z - 1].hasSearched)
                    {
                        // Suma un paso al siguiente tile
                        generator.terrainTiles[x, z - 1].steps = steps + 1;
                        // Indica al siguiente tile que busque al objetivo en sus vecinos
                        generator.terrainTiles[x, z - 1].search = true;
                    }
                }
            }
        }

        hasSearched = true;
        search = false;
    }
        
}

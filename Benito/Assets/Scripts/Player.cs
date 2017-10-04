using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

    public float movementSpeed = 10f;
    public float stepSpeed = 0.03f;

    [HideInInspector]
    public bool collidedFront, collidedBack, collidedLeft, collidedRight, pathFinded;

    public List<Vector3> positionsToTranslate = new List<Vector3>();

    private GameObject target;
    private bool move;
    private GameGenerator generator;
    private NavMeshAgent agent;
    private TerrainTile actualTile;

    public int i;
    private float stepTimer = 0;

	// Use this for initialization
	void Awake () {
        generator = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameGenerator>();
        agent = GetComponent<NavMeshAgent>();
	}

    private void Start()
    {
        i = 0;
        actualTile = generator.terrainTiles[0, 0];
    }

    // Update is called once per frame
    void Update () {
        CheckInput();
        Movement();
	}

    void CheckInput()
    {
        if(Input.touchCount > 0)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.GetTouch(0).position));

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.position != transform.position && hit.transform.tag == "Tile")
                {
                    target = hit.transform.gameObject;
                    move = true;
                }
            }

        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.position != transform.position && !move)
                {
                    //print("Click!");
                    target = hit.transform.gameObject;
                    generator.terrainTiles[hit.transform.GetComponent<TerrainTile>().x, hit.transform.GetComponent<TerrainTile>().z].target = true;
                    generator.terrainTiles[actualTile.x, actualTile.z].search = true;
                    move = true;
                }
            }
        }
    }

    void Movement()
    {
        if (move && pathFinded)
        {
            if (i >= 0)
            {
                // Temporizador de pasos
                stepTimer += Time.deltaTime;
                if(stepTimer >= movementSpeed)
                {
                    transform.position = new Vector3(positionsToTranslate[i].x, transform.position.y, positionsToTranslate[i].z);
                    if (i == 0)
                    {
                        i = 0;
                        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                        actualTile = target.GetComponent<TerrainTile>();
                        move = false;
                        pathFinded = false;
                        generator.reverseSearch = false;
                        foreach (TerrainTile tile in generator.terrainTiles)
                        {
                            tile.chainPositions.Clear();
                            tile.target = false;
                            tile.search = false;
                            tile.hasSearched = false;
                            tile.reverse = false;
                            tile.backwarded = false;
                            tile.steps = 0;
                        }
                    }
                    else
                    {
                        i--;
                        //print(i);
                        stepTimer = 0;
                    }
                    
                }

                
            }
        }
    }
}

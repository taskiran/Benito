using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour {

    public int x, y, z;

    private GameGenerator generator;
    private Player player;

    private bool positionated = false;

    private void Awake()
    {
        generator = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameGenerator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (!positionated)
            PositionateTile();
    }

    void PositionateTile()
    {
        if (transform.position.y > -0.5f)
        {
            transform.Translate(Vector3.down * 40 * Time.deltaTime);
        }
        if (transform.position.y < -0.5f)
        {
            transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            positionated = true;
        }
    }
        
}

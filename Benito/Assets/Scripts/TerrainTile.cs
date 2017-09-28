using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour {

    public int x, y, z;

    private void Update()
    {
        if(transform.position.y > 0)
        {
            transform.Translate(Vector3.down * 40 * Time.deltaTime);
        }
        if(transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
}

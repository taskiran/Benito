using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour {

    public GameObject target = null;

    private float xOffset, yOffset, zOffset;

	// Use this for initialization
	void Start () {
        xOffset = transform.position.x;
        yOffset = transform.position.y;
        zOffset = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(target.transform.position.x + xOffset, target.transform.position.y + yOffset, target.transform.position.z + zOffset);
	}
}

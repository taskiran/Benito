using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObject : MonoBehaviour {

    public float rotationSpeed = 8f;

    private Camera cam;
    private bool clickedOnThisObject = false;
	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && clickedOnThisObject)
        {
            Rotation();
        }
		
	}

    void Rotation()
    {
        transform.Rotate(cam.transform.right, Input.GetAxis("Mouse Y") * rotationSpeed, Space.World);
        transform.Rotate(-cam.transform.up, Input.GetAxis("Mouse X") * rotationSpeed, Space.World);
    }

    private void OnMouseDown()
    {
        clickedOnThisObject = true;
    }

    private void OnMouseUp()
    {
        clickedOnThisObject = false;
    }
}

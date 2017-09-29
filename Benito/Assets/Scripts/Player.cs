using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float movementSpeed = 10f;
    public float stepSpeed = 0.03f;

    [HideInInspector]
    public bool collidedFront, collidedBack, collidedLeft, collidedRight;

    private Vector3 movementVector;
    private bool move, xMoved;
    private GameGenerator generator;

	// Use this for initialization
	void Start () {
        generator = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameGenerator>();
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
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.position != transform.position)
                {
                    movementVector = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                    xMoved = false;
                    move = true;
                }
            }
        }
    }

    void Movement()
    {
        if(move)
        {
            // Si la x es menor a la suya
            //print(transform.position.x + ", " + movementVector.x);
            if(movementVector.x < transform.position.x)
            {
                InvokeRepeating("MoveMX", 0f, stepSpeed);
            }

            // Si la x es mayor a la suya
            if (movementVector.x > transform.position.x)
            {
                InvokeRepeating("MovePX", 0f, stepSpeed);
            }


            // Si ha llegado a la X de destino
            if (xMoved)
            {
                // Si la z es menor a la suya
                if (movementVector.z < transform.position.z)
                {
                    InvokeRepeating("MoveMZ", 0, stepSpeed);
                }

                // Si la z es mayor a la suya
                if (movementVector.z > transform.position.z)
                {
                    InvokeRepeating("MovePZ", 0, stepSpeed);
                }
            }
        }
    }

    // Move to front
    void MoveMX()
    {
        if (collidedFront)
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            collidedFront = false;
            CancelInvoke();
        }


        if(transform.position.x < movementVector.x)
        {
            transform.position = new Vector3(movementVector.x, transform.position.y, transform.position.z);
            xMoved = true;
            CancelInvoke();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), movementSpeed * Time.deltaTime);
        }
    }
    void MovePX()
    {
        if (transform.position.x > movementVector.x)
        {
            transform.position = new Vector3(movementVector.x, transform.position.y, transform.position.z);
            xMoved = true;
            CancelInvoke();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), movementSpeed * Time.deltaTime);
        }
    }
    void MoveMZ()
    {
        if (transform.position.z < movementVector.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, movementVector.z);
            move = false;
            CancelInvoke();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), movementSpeed * Time.deltaTime);
        }
    }
    void MovePZ()
    {
        if (transform.position.z > movementVector.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, movementVector.z);
            move = false;
            CancelInvoke();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), movementSpeed * Time.deltaTime);
        }
    }
}

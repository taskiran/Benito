using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float movementSpeed = 10f;

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
                    move = true;
                }
            }
        }
    }

    void Movement()
    {
        if(move)
        {
            /*transform.position = Vector3.MoveTowards(transform.position, movementVector, movementSpeed * Time.deltaTime);*/

            if(movementVector.x < transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), movementSpeed * Time.deltaTime);
            }
            else if (movementVector.x > transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), movementSpeed * Time.deltaTime);
            }

            if (movementVector.z < transform.position.z && xMoved)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), movementSpeed * Time.deltaTime);
            }
            else if (movementVector.z > transform.position.z)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), movementSpeed * Time.deltaTime);
            }

            if(transform.position.x == movementVector.x)
            {
                print("1");
                transform.position = new Vector3(movementVector.x, transform.position.y, transform.position.z);
                xMoved = true;
            }
            else
            {
                xMoved = false;
            }

            if (transform.position == movementVector)
            {
                move = false;
            }
        }
    }
}

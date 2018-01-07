using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alumno : MonoBehaviour {
    public float waitTime = 10f;

    [HideInInspector]
    public bool returnToStairs;
    [HideInInspector]
    public Vector3 stairsPos;

    private GameObject garita;
    private NavMeshAgent agent;
    private MainGameManager gm;
    private bool onGarita;
    private float timer;
    
	// Use this for initialization
	void Start () {
        gm = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>();
        garita = GameObject.FindGameObjectWithTag("Garita");
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(garita.transform.position);
        onGarita = false;
        timer = waitTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (onGarita)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                Angry();
            }
        }


        if (returnToStairs)
        {
            agent.SetDestination(stairsPos);
            transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
            float dist = Vector3.Distance(transform.position, stairsPos);
            if(dist < 1f)
            {
                gm.alumnos.Remove(gameObject);
                Destroy(gameObject);
            }
        }
	}

    void Angry()
    {
        returnToStairs = true;
    }

    private void OnEnable()
    {
        garita = GameObject.FindGameObjectWithTag("Garita");
        if(garita != null && !returnToStairs)
            agent.SetDestination(garita.transform.position);
        else if (returnToStairs)
            agent.SetDestination(stairsPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Garita" && agent.destination != stairsPos)
        {
            gm.numberOfAlumnosEnGarita++;
            onGarita = true;
        }
            
    }
}

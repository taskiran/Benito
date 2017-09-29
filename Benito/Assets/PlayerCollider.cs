using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour {

    private Player player;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        print(transform.gameObject.name);
        switch (transform.gameObject.name)
        {
            case ("Front"):
                player.collidedFront = true;
                break;

            default:
                break;
        }
    }
}

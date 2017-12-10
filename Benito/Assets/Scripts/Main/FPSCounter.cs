using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    public Text fpsText;

    float deltaTime = 0f;
    float fps = 0f;
    float msec = 0f;
    string text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        fpsText.text = text;
	}
}

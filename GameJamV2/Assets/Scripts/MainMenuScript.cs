﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void StartGame ()
    {
        Application.LoadLevel("SampleScene");
    }
    
    public void QuitGame ()
    {
        Application.Quit();
    }
    
    // Update is called once per frame
	void Update () {
		
	}
}

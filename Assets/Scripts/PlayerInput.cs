using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public int Direction = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.W))
            Direction = 0;
        if (Input.GetKeyDown(KeyCode.D))
            Direction = 1;
        if (Input.GetKeyDown(KeyCode.A))
            Direction = 2;
	}
}

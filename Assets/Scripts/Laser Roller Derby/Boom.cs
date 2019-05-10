using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour {
    float scale = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        scale += Time.deltaTime;
        if (scale > 1.0)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            transform.position += new Vector3(0, Time.deltaTime, 0);
            transform.localScale += new Vector3(4*Time.deltaTime, 4*Time.deltaTime, 4*Time.deltaTime);
        }
	}
}

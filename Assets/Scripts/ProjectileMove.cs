using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour {

    public float speed;
    

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("no speed");
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blendTreeTest : MonoBehaviour {
	private Animator animPerso; // Animator du joueur

	
	// Use this for initialization
	void Start () {
		animPerso = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(animPerso == null) return;

		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		deplacement(x,y);
	}

	private void deplacement(float x, float y){
		animPerso.SetFloat("VelX",x);
		animPerso.SetFloat("VelY",y);
		
	}
}

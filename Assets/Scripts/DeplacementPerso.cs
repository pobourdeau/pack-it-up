using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPerso : MonoBehaviour {

    private Rigidbody rbPerso;
    private Animator animPerso;

    public float vitesseDeplacement = 10f;
    public float vDeplacement;
    public float vRotation;


	// Use this for initialization
	void Start () {
        rbPerso = GetComponent<Rigidbody>();
        animPerso = GetComponent<Animator>();
	}

    void FixedUpdate() {
        transform.Rotate(0, Input.GetAxis("Horizontal") * vRotation, 0);

        vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;

        print(vDeplacement);

        rbPerso.velocity = (transform.forward * vDeplacement) + new Vector3(0, rbPerso.velocity.y, 0);


    }
}

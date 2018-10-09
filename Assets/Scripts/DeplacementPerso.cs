using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPerso : MonoBehaviour {

    private Rigidbody rbPerso;
    private Animator animPerso;

    public float vitesseDeplacement = 10f;
    public float vDeplacement;
    public float vRotation;

    public GameObject txtRecolter;
    public int[] aInventaire;
    // Bois
    // Fer
    // Cuir


	// Use this for initialization
	void Start () {
        rbPerso = GetComponent<Rigidbody>();
        animPerso = GetComponent<Animator>();
        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;
	}

    void FixedUpdate() {
        transform.Rotate(0, Input.GetAxis("Horizontal") * vRotation, 0);

        vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;

        rbPerso.velocity = (transform.forward * vDeplacement) + new Vector3(0, rbPerso.velocity.y, 0);   
    }

    void OnTriggerEnter(Collider objCollider) {
        txtRecolter.SetActive(true);
    }

    private void OnTriggerStay(Collider objCollider) {
        if (Input.GetMouseButtonDown(0)) {

            if(objCollider.gameObject.tag == "bois") {
                aInventaire[0] = 1;
            }
            if (objCollider.gameObject.tag == "fer") {
                aInventaire[1] = 1;
            }
            if (objCollider.gameObject.tag == "cuir") {
                aInventaire[2] = 1;
            }

            objCollider.gameObject.SetActive(false);
            txtRecolter.SetActive(false);
        }
    }

    void OnTriggerExit(Collider objCollider) {
        txtRecolter.SetActive(false);
    }
}

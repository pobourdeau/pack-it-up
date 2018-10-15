using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeplacementPerso : MonoBehaviour {

    private Rigidbody rbPerso;
    private Animator animPerso;

    public float vitesseDeplacement = 10f;
    public float vDeplacement;
    public float vRotation;

    public GameObject txtRecolter;
    public int[] aInventaire;
    // aInventaire[0] = Bois
    // aInventaire[1] = Fer
    // aInventaire[2] = Cuir
    public GameObject[] aCrochetInv;
    // aCrochetInv[0] = crochet Bois
    // aCrochetInv[1] = crochet Fer
    // aCrochetInv[2] = crochet Cuir


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

        if (objCollider.gameObject.tag == "bois") {
            if(aInventaire[0] < 1) {
                txtRecolter.SetActive(true);
            }
        }
        if (objCollider.gameObject.tag == "fer") {
            if (aInventaire[1] < 1) {
                txtRecolter.SetActive(true);
            }
        }
        if (objCollider.gameObject.tag == "cuir") {
            if (aInventaire[2] < 1) {
                txtRecolter.SetActive(true);
            }
        }

    }

    private void OnTriggerStay(Collider objCollider) {
        if (Input.GetMouseButtonDown(0)) {

            if(objCollider.gameObject.tag == "bois" && aInventaire[0] < 1) {
                aInventaire[0] = 1;
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[0].SetActive(true);
            }
            if (objCollider.gameObject.tag == "fer" && aInventaire[1] < 1) {
                aInventaire[1] = 1;
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[1].SetActive(true);
            }
            if (objCollider.gameObject.tag == "cuir" && aInventaire[2] < 1) {
                aInventaire[2] = 1;
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[2].SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider objCollider) {
        txtRecolter.SetActive(false);
    }
}

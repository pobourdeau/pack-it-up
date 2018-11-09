using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoirATravers : MonoBehaviour {

    public Material transparent;
    public Material normal;

    void OnTriggerStay(Collider infoObj) {
        if(infoObj.gameObject.tag == "Player") {
            GetComponent<Renderer>().material = transparent;
        }
    }

    void OnTriggerExit(Collider infoObj) {
        if (infoObj.gameObject.tag == "player") {
            GetComponent<Renderer>().material = normal;
        }
    }
}

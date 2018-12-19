using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileMove : MonoBehaviour {

    public float speed;
    public float fireRate;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
	}

    /**
     * Détruire la boule de fin quand elle touche quelqu'un
     * @param Collision collision
     * @return void
     */
    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "knight" || collision.gameObject.tag == "mage") {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

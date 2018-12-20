using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileMove : MonoBehaviour {

    public float speed; //Valeur float pour la vitesse de la boule de feu
    public float fireRate; //Fréquence è laquelle on peut tirer la boule de feu

	// Use this for initialization
	void Start () {
		
	}

    // Update la position de la boule de feu
    void Update() {
        if (speed != 0)
        {
            //La boule de feu est propulsé vers l'avant avec une vitesse float
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
	}

    /**
     * Détruire la boule de fin quand elle touche quelqu'un
     * 
     * @param Collision collision
     * @return void
     * @author Pier-Olivier Bourdeau et Vincent Gagnon
     */
    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "knight" || collision.gameObject.tag == "mage") {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

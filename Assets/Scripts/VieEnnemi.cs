using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TEMPORAIRE...

public class VieEnnemi : MonoBehaviour {

    public GameObject oJoueur;

    /**
     * Si il y a des collision avec les objets 
     * @param void
     * @return void
     * 
     * Pier-Olivier Bourdeau
     */
    void OnTriggerEnter(Collider infoObj) {

        // Si le joueur attaque,
        if(oJoueur.GetComponent<DeplacementPerso>().attaque == true) {
            // Si l'objet est l'arme
            if (infoObj.gameObject.tag == "arme") {
                // Désactiver l'ennemi
                this.gameObject.SetActive(false);
            }
        }
    }
}

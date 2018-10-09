using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ================================================ */
// Script de déplacement de la caméra du personnage
// Déplacer la caméra du personnage
// Auteur: Pier-Olivier Bourdeau
// Dernière modification : 2018-10-09
/* ================================================ */

public class DeplacementCam : MonoBehaviour {

    public GameObject cible; // La cible à suivre
    public Vector3 distanceCamera; // La postion de la caméra
	
	// Update is called once per frame
	void Update () {
        // Déplacer la caméra en fonction de la cible
		transform.position = distanceCamera + cible.transform.position;
	}
}

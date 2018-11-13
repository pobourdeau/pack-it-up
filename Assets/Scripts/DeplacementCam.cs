using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Déplacement de la caméra pour qu'elle suive le personnage en tout temps
 * @author Pier-Olivier Bourdeau
 * @version 2018-11-12
 */
public class DeplacementCam : MonoBehaviour {

    public GameObject cible; // La cible à suivre
    public Vector3 distanceCamera; // La postion de la caméra
	
    /**
     * Déplacer la caméra en fonction de la cible
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
	void Update () {
        // Déplacer la caméra en fonction de la cible
		transform.position = distanceCamera + cible.transform.position;
	}
}

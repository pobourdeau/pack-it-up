using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


/**
 * Gestionnaire de vidéo d'intro : Lancer la vidéo en ouverture de jeu
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-11-12
 */
public class gestionnaireVideo : MonoBehaviour {

    public GameObject cam; // Caméra de la vidéo
    public VideoPlayer video; // Vidéo
    public GameObject oVideo; // Objet qui contient la vidéo
    public GameObject oCamPrinc; // Caméra principal
    public GameObject menu; // Menu principal
    public GameObject gestionMenu; // Gestionnaire du menu principal

	/**
     * Au commencement du jeu
     * @param void
     * @return void
     */
	void Start () {
        // Si c'est le premier lancement du jeu
        if(MenuPrinc.premierLancement == true) {
            // Lancer la vidéo et vérifier si elle joue à chaque seconde
            Invoke("LancerVideo", 0);
            InvokeRepeating("VerifierVideo", 2f, 1f);
        }
        else {
            gestionMenu.SetActive(true);
            menu.SetActive(true);
        }
    }

    /**
     * Lancer la vidéo 
     * @param void
     * @return void
     */
    public void LancerVideo() {
        video.Play();
    }

    /**
     * Vérifier si la vidéo est entrain de jouer
     * @param void
     * @return void
     */
    public void VerifierVideo() {
        // Si la vidéo ne joue pas,
        if(video.isPlaying == false) {
            // Mettre en pause
            video.Pause();
            CancelInvoke("VerifierVideo");

            // Fermer la vidéo et aller au menu principal
            cam.SetActive(false);
            oCamPrinc.SetActive(true);
            oVideo.SetActive(false);
            gestionMenu.SetActive(true);
            menu.SetActive(true);
        }
    }
}

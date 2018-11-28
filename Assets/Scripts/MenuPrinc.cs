using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Gestionnaire du menu principal
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-11-12
 */
public class MenuPrinc : MonoBehaviour {

    // Menu principal
    public GameObject imgDebut; // Image du début
    public static bool premierLancement = true; // Est-ce le premier lancement du jeu?
    public GameObject menuPrinc; // Objet MenuPrinc
    public Button btnQuitter; // Bouton Quitter

    /**
     * Ajout d'événement sur tous les boutons du menu
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    void Start() {

        // Menu principal
        btnQuitter.onClick.AddListener(() => QuitterJeu());
    }


    void Update() {
        // Si c'est le premier lancement,
        if (premierLancement) {
            // Appuyer sur la barre d'espace pour ouvrir le menu
            if (Input.GetKeyDown(KeyCode.Space)) {
                premierLancement = false;
                imgDebut.SetActive(false);
                menuPrinc.SetActive(true);
            }
        }
        else {
            // Ouvrir le menu
            imgDebut.SetActive(false);
            menuPrinc.SetActive(true);
        }

    }


    /**
     * Ouvrir le menu pour lancer une partie
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void OuvrirMenuJouer() {
        SceneManager.LoadScene("SceneRejoindre");
    }


    /**
     * Quitter le jeu
     * @param : void
     * @return : void
     * @author Issam Aloulou
     */
    public void QuitterJeu() {
        Application.Quit();
    }
}

﻿using System.Collections;
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
    public Button btnJouer; // Bouton Jouer
    public Button btnAide; // Bouton d'aide
    public Button btnReglages; // Bouton des réglages

    // Menu jouer
    public GameObject menuJouer; // Objet du menu pour lancer une partie
    public InputField txtCreer; // Inputfield pour créer une partie
    public Button btnCreerPartie; // Bouton pour créer une partie
    public InputField txtRejoindre; // Inputfield pour rejoindre une partie
    public Button btnRejoindrePartie; // Bouton pour rejoindre une partie
    public Toggle cchPretJ1; // Case à cocher Prêt? du Joueur 1
    public Toggle cchPretJ2; // Case à cocher Prêt? du Joueur 2
    public Toggle cchPretJ3; // Case à cocher Prêt? du Joueur 3
    public Toggle cchPretJ4; // Case à cocher Prêt? du Joueur 4
    public Button btnCommencer; // Bouton commencer la partie

    // Menu aide
    public GameObject menuAide; // Objet menu aide
    public GameObject menuReglages; // Objet des réglages
    public GameObject menuPerso; // Objet Menu personnage

    /**
     * Ajout d'événement sur tous les boutons du menu
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    void Start() {

        // Menu principal
        btnQuitter.onClick.AddListener(() => QuitterJeu());
        btnJouer.onClick.AddListener(() => OuvrirMenuJouer());
        btnAide.onClick.AddListener(() => OuvrirMenuAide());
        btnReglages.onClick.AddListener(() => OuvrirMenuReglages());

        // Menu Jouer
        btnCreerPartie.onClick.AddListener(() => CreerPartie());
        btnRejoindrePartie.onClick.AddListener(() => RejoindrePartie());
        btnCommencer.onClick.AddListener(() => CommencerPartie());
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


            // Si l'input field de nom de partie ne contient rien,
            if (txtCreer.text != "") {
                // Activer le bouton de création de partie
                btnCreerPartie.interactable = true;
            }
            else {
                // Désactiver le bouton de création de partie
                btnCreerPartie.interactable = false;
            }

            // Si l'input field de nom de partie ne contient rien,
            if (txtRejoindre.text != "") {
                // Activer le bouton de création de partie
                btnRejoindrePartie.interactable = true;
            }
            else {
                // Désactiver le bouton de création de partie
                btnRejoindrePartie.interactable = false;
            }

            if (cchPretJ1.isOn && cchPretJ2.isOn && cchPretJ3.isOn && cchPretJ4.isOn) {
                btnCommencer.interactable = true;
            }
            else {
                btnCommencer.interactable = false;
            }

    }


    /**
     * Ouvrir le menu pour lancer une partie
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void OuvrirMenuJouer() {
        menuPrinc.SetActive(false);
        menuJouer.SetActive(true);
    }


    /**
     * Ouvrir le menu d'aide
     * @param void
     * @return void
     * @author Vincent Gagnon
     */
    public void OuvrirMenuAide() {
        menuPrinc.SetActive(false);
        menuAide.SetActive(true);
    }


    /**
     * Ouvrir le menu des réglages
     * @param void
     * @return void
     */
    public void OuvrirMenuReglages() {

    }


    /**
     * Ouvrir le menu pour créer une partie
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void CreerPartie() {
        // Créer la partie (Network Manager)

        // Ouvrir le menu de choix de personnage
        menuJouer.SetActive(false);
        menuPerso.SetActive(true);
    }


    /**
     * Ouvrir le menu pour rejoindre une partie
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void RejoindrePartie() {
        // Créer la partie (Network Manager)

        // Ouvrir le menu de choix de personnage
        menuJouer.SetActive(false);
        menuPerso.SetActive(true);
    }


    /**
     * Lancer la scène de jeu
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void CommencerPartie() {
        SceneManager.LoadScene("SceneJeu");
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

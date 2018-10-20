using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* ================================== */
// Script du menu principal
// Gestionnaire des boutons
// Auteur: Pier-Olivier Bourdeau
// Dernière modification : 2018-10-19
/* ================================== */

public class MenuPrinc : MonoBehaviour {

    // Menu principal
    public GameObject menuPrinc;
    public Button btnQuitter;
    public Button btnJouer;
    public Button btnAide;
    public Button btnReglages;

    // Menu jouer
    public GameObject menuJouer;
    public InputField txtCreer;
    public Button btnCreerPartie;
    public InputField txtRejoindre;
    public Button btnRejoindrePartie;
    public Toggle cchPretJ1;
    public Toggle cchPretJ2;
    public Toggle cchPretJ3;
    public Toggle cchPretJ4;
    public Button btnCommencer;

    public GameObject menuAide;
    public GameObject menuReglages;
    public GameObject menuPerso;


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

        if(cchPretJ1.isOn && cchPretJ2.isOn && cchPretJ3.isOn && cchPretJ4.isOn) {
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
     */
    public void OuvrirMenuJouer() {
        menuPrinc.SetActive(false);
        menuJouer.SetActive(true);
    }


    /**
     * Ouvrir le menu d'aide
     * @param void
     * @return void
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
     */
    public void CommencerPartie() {
        SceneManager.LoadScene("SceneJeu");
    }


    /**
     * Quitter le jeu
     * @param : void
     * @return : void
     */
    public void QuitterJeu() {
        Application.Quit();
    }
}

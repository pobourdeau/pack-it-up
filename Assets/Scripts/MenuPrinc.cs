using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* ================================== */
// Script du menu principal
// Gestionnaire des boutons
// Auteur: Pier-Olivier Bourdeau
// Dernière modification : 2018-10-09
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

    }

    public void OuvrirMenuJouer() {
        menuPrinc.SetActive(false);
        menuJouer.SetActive(true);
    }

    public void OuvrirMenuAide() {

    }

    public void OuvrirMenuReglages() {

    }

    public void CreerPartie() {

        // Créer la partie (Network Manager)

        // Ouvrir le menu de choix de personnage
        menuJouer.SetActive(false);
        menuPerso.SetActive(true);
        
        
    }

    public void RejoindrePartie() {
        // Créer la partie (Network Manager)

        // Ouvrir le menu de choix de personnage
        menuJouer.SetActive(false);
        menuPerso.SetActive(true);
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

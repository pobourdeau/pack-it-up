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

    public GameObject BtnQuitter;
    public GameObject BtnHeberger;
    public GameObject BtnRejoindre;
    public GameObject BtnAide;
    public GameObject BtnReglages;
    public GameObject BtnRetour;
    public GameObject NetworkManager;

    /**
     * Ouvrir la scène pour rejoindre une partie
     * @param : void
     * @return : void
     */
    public void LancerSceneRejoindre() {
        BtnHeberger.SetActive(false);
        BtnRejoindre.SetActive(false);
        BtnAide.SetActive(false);
        BtnReglages.SetActive(false);

        NetworkManager.SetActive(true);
        BtnRetour.SetActive(true);
    }


    /**
     * Ouvrir la scène pour créer une nouvelle partie 
     * @param : void
     * @return : void
     */
    public void LancerSceneHeberger() {
        BtnHeberger.SetActive(false);
        BtnRejoindre.SetActive(false);
        BtnAide.SetActive(false);
        BtnReglages.SetActive(false);

        NetworkManager.SetActive(true);
    }


    /**
     * Ouvrir la scène d'aide
     * @param : void
     * @return : void
     */
    public void LancerSceneAide() {

    }


    /**
     * Ouvrir la scène des réglages du jeu
     * @param : void
     * @return : void
     */
    public void LancerSceneReglages() {

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

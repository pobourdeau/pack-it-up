using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/**
 * Gestion de l'attribution des joueurs
 * @author Pier-Olivier Bourdeau
 * @version 2018-11-12
 */

[RequireComponent(typeof(InputField))]
public class NomJoueur : MonoBehaviour {

    const string pseudoJoueurPrefKey = "PlayerName";
    InputField txtPseudoJoueur;//zone nom joueur
    public Text lblNomJoueur;//nom joueur
    public Button btnConnexion;//Bouton de connexion au serveur

    // Use this for initialization
    void Start () {
        string pseudoDefaut = string.Empty;
        //La zone de texte pour le nom
        txtPseudoJoueur = this.GetComponent<InputField>();

        //vide
        lblNomJoueur.text = "";

        if (txtPseudoJoueur != null) {

            if (PlayerPrefs.HasKey(pseudoJoueurPrefKey)) {
                pseudoDefaut = PlayerPrefs.GetString(pseudoJoueurPrefKey);
                txtPseudoJoueur.text = pseudoDefaut;
                //Attribuer le nom de base
                lblNomJoueur.text = pseudoDefaut;
            }
        }
        else {
            //Empecher de se connecter
            btnConnexion.interactable = false;
        }

        //Nom de base donc "joueur" + 4 chiffres aux hazard qui sont disponnible... ex: joueur 3428
        PhotonNetwork.NickName = pseudoDefaut;
	}
	

    public void SetPseudoJoueur(string value) {
        if (string.IsNullOrEmpty(value)) {
            return;
        }

        PhotonNetwork.NickName = value;
        lblNomJoueur.text = value;

        PlayerPrefs.SetString(pseudoJoueurPrefKey, value);

        btnConnexion.interactable = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]
public class NomJoueur : MonoBehaviour {

    const string pseudoJoueurPrefKey = "PlayerName";
    InputField txtPseudoJoueur;
    public Text lblNomJoueur;
    public Button btnConnexion;

    // Use this for initialization
    void Start () {
        string pseudoDefaut = string.Empty;
        txtPseudoJoueur = this.GetComponent<InputField>();

        lblNomJoueur.text = "";

        if (txtPseudoJoueur != null) {

            if (PlayerPrefs.HasKey(pseudoJoueurPrefKey)) {
                pseudoDefaut = PlayerPrefs.GetString(pseudoJoueurPrefKey);
                txtPseudoJoueur.text = pseudoDefaut;
                lblNomJoueur.text = pseudoDefaut;
            }
        }
        else {
            btnConnexion.interactable = false;
        }

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

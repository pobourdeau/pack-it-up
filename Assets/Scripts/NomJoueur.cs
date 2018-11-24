using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]
public class NomJoueur : MonoBehaviour {

    const string pseudoJoueurPrefKey = "PlayerName";
    public Button btnChoisirNom;
    InputField txtPseudoJoueur;

    // Use this for initialization
    void Start () {
        string pseudoDefaut = string.Empty;
        txtPseudoJoueur = this.GetComponent<InputField>();

        if (txtPseudoJoueur != null) {

            if (PlayerPrefs.HasKey(pseudoJoueurPrefKey)) {
                pseudoDefaut = PlayerPrefs.GetString(pseudoJoueurPrefKey);
                txtPseudoJoueur.text = pseudoDefaut;
            }
        }

        PhotonNetwork.NickName = pseudoDefaut;
	}
	

    public void SetPseudoJoueur(string value) {
        if (string.IsNullOrEmpty(value)) {
            return;
        }

        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(pseudoJoueurPrefKey, value);
    }


    void FixedUpdate() {
        if(txtPseudoJoueur.text == "") {
            btnChoisirNom.interactable = false;
        }
        else {
            btnChoisirNom.interactable = true;
        }
    }
}

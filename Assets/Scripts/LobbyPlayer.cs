using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer {

    public GameObject parentPref;
    public Toggle cchPret;
    public Text txtJoueur;

    public override void OnClientEnterLobby() {
        base.OnClientEnterLobby();

        parentPref = GameObject.FindGameObjectWithTag("lobbyParent");
        gameObject.transform.SetParent(parentPref.transform);
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        if (isLocalPlayer) {
            cchPret.enabled = true;
            txtJoueur.text = "Mon joueur";
        }
        else {
            cchPret.enabled = false;
            txtJoueur.text = "Ennemi";
        }
    }

    public void EstPret() {
        SendReadyToBeginMessage();
        
    }

}

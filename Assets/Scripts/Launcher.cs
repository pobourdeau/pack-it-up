using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    string gameVersion = "1";
    [SerializeField]
    private byte nbMaxDeJoueur = 4;
    [SerializeField]
    private GameObject panConnexion;
    [SerializeField]
    private GameObject progressionConnexion;
    [SerializeField]
    private GameObject btnConnexion;


    bool estConnecte;

	// Use this for initialization
	void Start () {
        progressionConnexion.SetActive(false);
	}
	
	void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;
	}

    public void Connect() {
        estConnecte = true;

        progressionConnexion.SetActive(true);
        panConnexion.SetActive(false);

        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        }
        else {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        if (estConnecte) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = nbMaxDeJoueur });
    }

    public override void OnJoinedRoom() {
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel("SceneJeu");
        }
    }

    public override void OnDisconnected(DisconnectCause cause) {
        progressionConnexion.SetActive(false);
        panConnexion.SetActive(true);
    }

}

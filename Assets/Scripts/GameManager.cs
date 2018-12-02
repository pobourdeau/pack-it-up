using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager Instance;
    public GameObject[] persoPrefab;
    public GameObject[] aSpawnerPoints;
    public string nomPersoChoisi = "";
    public bool partieEnCours = false;
    public Text[] txtNomJoueur;
    public Text txtMaster;
    public Button btnLancer;
    public Text nbJoueurRoom;


    void Start() {
        Instance = this;

        for(int i=0; i<persoPrefab.Length; i++) {
            if(persoPrefab[i].name == GestionnaireLobby.choixPersoJoueur) {
                nomPersoChoisi = persoPrefab[i].name;
            }
        }

        if(nomPersoChoisi != "") {

            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            GameObject joueur = PhotonNetwork.Instantiate(nomPersoChoisi, aSpawnerPoints[PhotonNetwork.CurrentRoom.PlayerCount-1].transform.position, aSpawnerPoints[PhotonNetwork.CurrentRoom.PlayerCount-1].transform.rotation, 0);
            //joueur.SetActive(false);
        }

        print("AAA");

        if (PhotonNetwork.IsMasterClient) {
            txtMaster.text = PhotonNetwork.PlayerList[0].NickName;
        }
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("SceneRejoindre");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player nouveauJoueur) {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", nouveauJoueur.NickName);

        print("###");

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            txtMaster.text = nouveauJoueur.NickName;    
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            btnLancer.interactable = true;
        }

        print(nouveauJoueur.NickName);
        txtNomJoueur[PhotonNetwork.CurrentRoom.PlayerCount - 1].text = nouveauJoueur.NickName;
        txtNomJoueur[PhotonNetwork.CurrentRoom.PlayerCount - 1].enabled = true;
        nbJoueurRoom.text = "0" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public override void OnPlayerLeftRoom(Player nouveauJoueur) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", nouveauJoueur.NickName);

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
                btnLancer.interactable = false;
                print("PAS ASSEZ DE JOUEUR...");
            }

            nbJoueurRoom.text = "0" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            //SceneManager.LoadScene("SceneRejoindre");
        }
    }

    public void LancerLaPartie() {

        print("JE LANCE LA PARTIE!!");

    }
}

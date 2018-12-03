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

    public GameObject[] aSpawnerBois; // Tous les points de spawn du bois
    public GameObject[] aSpawnerFer; // Tous les points de spawn du fer
    public GameObject[] aSpawnerCuir; // Tous les points de spawn du cuir


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

        // Générer les ressources sur la carte
        GenererRessources();
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


    /**
     * Générer les ressources sur la carte aléatoirement
     * @param void
     * @return void
     * @author Vincent Gagnon
     */
    void GenererRessources() {
        // Générer aléatoirement l'emplacement du bois
        Shuffle(aSpawnerBois);

        for (int iBois = 0; iBois < aSpawnerBois.Length / 2; iBois++) {
            GameObject oCloneBois = PhotonNetwork.Instantiate("bois", aSpawnerBois[iBois].transform.position, Quaternion.Euler(aSpawnerBois[iBois].transform.eulerAngles));
            oCloneBois.transform.parent = aSpawnerBois[iBois].transform;
        }

        // Générer aléatoirement l'emplacement du fer
        Shuffle(aSpawnerFer);

        for (int iFer = 0; iFer < aSpawnerFer.Length / 2; iFer++) {
            GameObject oCloneFer = PhotonNetwork.Instantiate("fer", aSpawnerFer[iFer].transform.position, Quaternion.Euler(aSpawnerFer[iFer].transform.eulerAngles));
            oCloneFer.transform.parent = aSpawnerFer[iFer].transform;
        }

        // Générer aléatoirement l'emplacement du cuir
        Shuffle(aSpawnerCuir);

        for (int iCuir = 0; iCuir < aSpawnerCuir.Length / 2; iCuir++) {
            GameObject oCloneCuir = PhotonNetwork.Instantiate("cuir", aSpawnerCuir[iCuir].transform.position, Quaternion.Euler(aSpawnerCuir[iCuir].transform.eulerAngles));
            oCloneCuir.transform.parent = aSpawnerCuir[iCuir].transform;
        }

    }


    /**
     * Mélanger un tableau sans répétition et doublons
     * @param GameObject[] aTab
     * @return void
     */
    void Shuffle(GameObject[] aTab) {
        for (int t = 0; t < aTab.Length; t++) {
            GameObject tmp = aTab[t];
            int r = Random.Range(t, aTab.Length);
            aTab[t] = aTab[r];
            aTab[r] = tmp;
        }
    }
}

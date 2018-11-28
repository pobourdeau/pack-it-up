using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GestionnaireLobby : MonoBehaviourPunCallbacks {

    public GameObject btnPartie;
    public GameObject panPartie;
    public Sprite spritePartie_neutre;
    public Sprite spritePartie_actif;

    public GameObject btnProfil;
    public GameObject panProfil;
    public Sprite spriteProfil_neutre;
    public Sprite spriteProfil_actif;

    public GameObject btnReglages;
    public GameObject panReglages;
    public Sprite spriteReglages_neutre;
    public Sprite spriteReglages_actif;

    public Text txtNbJoueur;
    public Text txtStatut;

    public GameObject panJoueur;
    public GameObject[] aDrapeaux;

    // Use this for initialization
    void Start () {
        btnPartie.GetComponent<Button>().onClick.AddListener(() => AfficherPartie());
        btnProfil.GetComponent<Button>().onClick.AddListener(() => AfficherProfil());
        btnReglages.GetComponent<Button>().onClick.AddListener(() => AfficherReglages());

        // Créer les joueurs dans le lobby d'attente
        CreationJoueurAttente();
    }

    void AfficherPartie() {
        panProfil.SetActive(false);
        panReglages.SetActive(false);
        panPartie.SetActive(true);

        btnProfil.GetComponent<Image>().sprite = spriteProfil_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_neutre;
        btnPartie.GetComponent<Image>().sprite = spritePartie_actif;
    }

    void AfficherProfil() {
        panReglages.SetActive(false);
        panPartie.SetActive(false);
        panProfil.SetActive(true);

        btnPartie.GetComponent<Image>().sprite = spritePartie_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_neutre;
        btnProfil.GetComponent<Image>().sprite = spriteProfil_actif;
    }

    void AfficherReglages() {
        panProfil.SetActive(false); 
        panPartie.SetActive(false);
        panReglages.SetActive(true);

        btnPartie.GetComponent<Image>().sprite = spritePartie_neutre;
        btnProfil.GetComponent<Image>().sprite = spriteProfil_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_actif;

    }

    void CreationJoueurAttente() {
        GameObject oJoueur = PhotonNetwork.Instantiate(aDrapeaux[PhotonNetwork.CurrentRoom.PlayerCount-1].gameObject.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
        oJoueur.transform.parent = panJoueur.transform;
    }


    public override void OnPlayerEnteredRoom(Player nouveauJoueur) {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", nouveauJoueur.NickName);
        if (PhotonNetwork.IsMasterClient) {
            txtNbJoueur.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4";
        }

        if(PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            txtStatut.text = "Prêt à lancer une partie.";
        }
    }

    public override void OnPlayerLeftRoom(Player nouveauJoueur) {
        if (PhotonNetwork.IsMasterClient) {
            txtNbJoueur.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4";
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
            
            // Désactiver le bouton "Lancer la partie"
            txtStatut.text = "En attente d'autres joueurs...";
        }
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("SceneMenu");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

}

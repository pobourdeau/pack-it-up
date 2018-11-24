using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/**
 * Gestionnaire de jeu : Temps restant à la partie, génération des objets aléatoirement et génération des personnages à leur position de spawn.
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-11-12
 */
public class GestionnaireJeu : MonoBehaviourPunCallbacks {

    public int iSecondeRestante; // Seconde restante
    public int iMinuteRestante; // Minute restante
    private int iSecondeTotal;
    public Text txtTimer; // UI text où le temps est écrit
    public Image imgTimer; // Image du timer

    public GameObject oBois; // Prefab du bois
    public GameObject oFer; // Prefab du fer
    public GameObject oCuir; // Prefab du cuir
    public GameObject[] aSpawnerBois; // Tous les points de spawn du bois
    public GameObject[] aSpawnerFer; // Tous les points de spawn du fer
    public GameObject[] aSpawnerCuir; // Tous les points de spawn du cuir

    public GameObject oKnight; // Le joueur principal
    public GameObject oMagicien; // Le joueur secondaire
    public GameObject[] aSpawnerJoueur;  // Tous les points de spawn des joueurs

    void Start() {
        // Calculer le nombre de seconde total à la partie
        iSecondeTotal = iMinuteRestante * 60 + iSecondeRestante;

        // Appeler la fonction timer à chaque seconde
        InvokeRepeating("Timer", 0, 1f);

        // Générer les ressources aléatoirement sur la carte
        GenererRessources();

        // Générer les joueur
        //oKnight.transform.position = aSpawnerJoueur[0].transform.position;
        //oMagicien.transform.position = aSpawnerJoueur[1].transform.position;
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

        for(int iBois=0; iBois<aSpawnerBois.Length / 2; iBois++) {
            GameObject oCloneBois = Instantiate(oBois, aSpawnerBois[iBois].transform.position, Quaternion.Euler(aSpawnerBois[iBois].transform.eulerAngles), aSpawnerBois[iBois].transform);
        }

        // Générer aléatoirement l'emplacement du fer
        Shuffle(aSpawnerFer);

        for (int iFer = 0; iFer < aSpawnerFer.Length / 2; iFer++) {
            GameObject oCloneFer = Instantiate(oFer, aSpawnerFer[iFer].transform.position, Quaternion.Euler(aSpawnerFer[iFer].transform.eulerAngles), aSpawnerFer[iFer].transform);
        }

        // Générer aléatoirement l'emplacement du cuir
        Shuffle(aSpawnerCuir);

        for (int iCuir = 0; iCuir < aSpawnerCuir.Length / 2; iCuir++) {
            GameObject oCloneCuir = Instantiate(oCuir, aSpawnerCuir[iCuir].transform.position, Quaternion.Euler(aSpawnerCuir[iCuir].transform.eulerAngles), aSpawnerCuir[iCuir].transform);
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

    /**
     * Gérer le temps restant de la partie
     * @param void
     * @return void
     * @author Issam Aloulou
     */
    void Timer() {
        
        // S'il reste plus que 0 seconde, diminuer la seconde
        if (iSecondeRestante > 0) {
            iSecondeRestante--;
        }
        // Sinon, remettre 59 secondes et diminuer la minute
        else {
            iSecondeRestante = 59;
            iMinuteRestante--;
        }

        // Formatage du chronomètre
        if(iSecondeRestante < 10) {
            txtTimer.text = iMinuteRestante + ":0" + iSecondeRestante;
        }
        else {
            txtTimer.text = iMinuteRestante + ":" + iSecondeRestante;
        }

        imgTimer.fillAmount = (float)(iMinuteRestante * 60 + iSecondeRestante) / iSecondeTotal;

        // S'il ne reste plus de temps, annuler l'appel de la fonction
        if (iMinuteRestante == 0 && iSecondeRestante == 0) {
            CancelInvoke();
            //RetourMenuPrinc();
        }
    }

    /**
     * Retourner au menu principal
     * @param void
     * @return void
     */
    /*public override void OnLeftRoom() {
        SceneManager.LoadScene("SceneRejoindre");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player nouveauJoueur) {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", nouveauJoueur.NickName);
        if (PhotonNetwork.IsMasterClient) {
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player nouveauJoueur) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", nouveauJoueur.NickName);

        if (PhotonNetwork.IsMasterClient) {
            LoadArena();
        }
    }

    void LoadArena() {
        if (!PhotonNetwork.IsMasterClient) {
            
        }

        PhotonNetwork.LoadLevel("SceneJeu");

    }*/
}

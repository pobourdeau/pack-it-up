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

    public GameObject oKnight; // Le joueur principal
    public GameObject oMagicien; // Le joueur secondaire
    public GameObject[] aSpawnerJoueur;  // Tous les points de spawn des joueurs

    public GameObject[] aUI;

    void Start() {
        // Calculer le nombre de seconde total à la partie
        iSecondeTotal = iMinuteRestante * 60 + iSecondeRestante;

        // Appeler la fonction timer à chaque seconde
        InvokeRepeating("Timer", 0, 1f);

        // Générer les ressources aléatoirement sur la carte
        //GenererRessources();

        // Générer les joueur
        //oKnight.transform.position = aSpawnerJoueur[0].transform.position;
        //oMagicien.transform.position = aSpawnerJoueur[1].transform.position;

        aUI[0].SetActive(true);
        aUI[1].SetActive(false);
        aUI[2].SetActive(false);
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

            aUI[0].SetActive(false);
            aUI[1].SetActive(false);
            aUI[2].SetActive(true);
            //RetourMenuPrinc();
        }
    }
}

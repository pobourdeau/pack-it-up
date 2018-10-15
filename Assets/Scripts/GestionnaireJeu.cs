using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* ================================== */
// Script du gestionnaire de jeu
// Gérer le chronomètre de partie
// Auteur: Pier-Olivier Bourdeau
// Dernière modification : 2018-10-09
/* ================================== */

public class GestionnaireJeu : MonoBehaviour {

    public int iSecondeRestante; // Seconde restante
    public int iMinuteRestante; // Minute restante
    public Text txtTimer; // UI text où le temps est écrit

    void Start() {
        // Appeler la fonction timer à chaque seconde
        InvokeRepeating("Timer", 0, 1f);
    }

    /**
     * Gérer le temps restant de la partie
     * @param void
     * @return void
     */
    void Timer() {
        
        // S'il reste plus que 0 seconde, diminuer la seconde
        if(iSecondeRestante > 0) {
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

        // S'il ne reste plus de temps, annuler l'appel de la fonction
        if (iMinuteRestante == 0 && iSecondeRestante == 0) {
            CancelInvoke();
        }
    }

    public void OuvrirMenuDeJeu() {
        
    }
}

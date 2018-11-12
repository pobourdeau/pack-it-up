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
    private int iSecondeTotal;
    public Text txtTimer; // UI text où le temps est écrit
    public Image imgTimer;

    public GameObject oBois;
    public GameObject oFer;
    public GameObject oCuir;
    public GameObject[] aSpawnerBois;
    public GameObject[] aSpawnerFer;
    public GameObject[] aSpawnerCuir;

    private int lastNumber;

    void Start() {
        // Calculer le nombre de seconde total à la partie
        iSecondeTotal = iMinuteRestante * 60 + iSecondeRestante;

        // Appeler la fonction timer à chaque seconde
        InvokeRepeating("Timer", 0, 1f);

        GenererRessources();
    }

    void GenererRessources() {
        for(int iBois = 0; iBois<5; iBois++) {
            //print(GetRandom(0, aSpawnerBois.Length));
        }
    }

    int GetRandom(int min, int max) {
        int rand = Random.Range(min, max);
        while (rand == lastNumber)
            rand = Random.Range(min, max);
        lastNumber = rand;
        return rand;
    }

    /**
     * Gérer le temps restant de la partie
     * @param void
     * @return void
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
            RetourMenuPrinc();
        }
    }

    public void RetourMenuPrinc() {
        SceneManager.LoadScene("SceneMenu");
    }
}

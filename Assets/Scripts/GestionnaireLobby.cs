using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GestionnaireLobby : MonoBehaviour {

    public GameObject btnPartie;//Bouton pour parties
    public GameObject panPartie;//Panneau pour parties
    public Sprite spritePartie_neutre;// Le sprite normal de partie
    public Sprite spritePartie_actif;//Le sprite actif de partie

    public GameObject btnProfil;//Bouton de profil
    public GameObject panProfil;//Panneau de profil
    public Sprite spriteProfil_neutre;// Le sprite normal de profil
    public Sprite spriteProfil_actif;//Le sprite actif de profil

    public GameObject btnReglages;//Le bouton pour les réglages
    public GameObject panReglages;//Le panneau pour la page des réglages
    public Sprite spriteReglages_neutre;// Le sprite normal de réglage
    public Sprite spriteReglages_actif;// Le sprite actif de réglage

    public Toggle cchKnight;//Choix du chevalier dans la page de choix de personnage
    public Toggle cchMage;//Choix du mage dans la page de choix de personnage

    public Image imgPerso;//L'image du personnage actuel
    public Sprite sprite_knight;//Le sprite du chevalier
    public Sprite sprite_mage;//Le sprite du mage

    public static string choixPersoJoueur = "knight";//Le choix de base est le chevalier


    /**
* Start
* @param void
* @return void
* 
* Pier-Olivier Bourdeau et Vincent
*/
    void Start () {
        //Ajout des fonction quand l'on clique sur un bouton du menu
        btnPartie.GetComponent<Button>().onClick.AddListener(() => AfficherPartie());
        btnProfil.GetComponent<Button>().onClick.AddListener(() => AfficherProfil());
        btnReglages.GetComponent<Button>().onClick.AddListener(() => AfficherReglages());


        //Quand on change de personnage à partir du chevalier
        cchKnight.onValueChanged.AddListener(delegate {
            //si le choix est actuellement le chevalier
            if (cchKnight.isOn) {
                //désactivation du choix du mage
                cchMage.isOn = false;
                //image du chevalier apparaît
                imgPerso.sprite = sprite_knight;
                //Le chevalier est choisi
                choixPersoJoueur = "knight";
            }
            //si le choix est actuellement le mage
            else
            {
                //désactivation du choix du guerrier
                cchMage.isOn = true;
                //image du magicien apparaît
                imgPerso.sprite = sprite_mage;
                //Le mage est choisi
                choixPersoJoueur = "mage";
            }
        });

        //Quand on change de personnage à partir du magicien
        cchMage.onValueChanged.AddListener(delegate {
            //Même principe ici que en haut
            if (cchMage.isOn) {
                cchKnight.isOn = false;
                imgPerso.sprite = sprite_mage;
                choixPersoJoueur = "mage";
            }
            else {
                cchKnight.isOn = true;
                imgPerso.sprite = sprite_knight;
                choixPersoJoueur = "knight";
            }
        });
    }

   /**
*Afficher la page de partie
* @param void
* @return void
* 
* Pier-Olivier Bourdeau
*/
    void AfficherPartie() {
        //Activation/désactivation des pages au besoin
        panProfil.SetActive(false);
        panReglages.SetActive(false);
        panPartie.SetActive(true);

        //Utilisation des sprites nécéssaires
        btnProfil.GetComponent<Image>().sprite = spriteProfil_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_neutre;
        btnPartie.GetComponent<Image>().sprite = spritePartie_actif;
    }

   /**
*Afficher la page de profil
* @param void
* @return void
* 
* Pier-Olivier Bourdeau
*/
    void AfficherProfil() {
        //Activation/désactivation des pages au besoin
        panReglages.SetActive(false);
        panPartie.SetActive(false);
        panProfil.SetActive(true);

        //Utilisation des sprites nécéssaires
        btnPartie.GetComponent<Image>().sprite = spritePartie_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_neutre;
        btnProfil.GetComponent<Image>().sprite = spriteProfil_actif;
    }

    /**
*Afficher la page de reglages
* @param void
* @return void
* 
* Pier-Olivier Bourdeau
*/
    void AfficherReglages() {
        //Activation/désactivation des pages au besoin
        panProfil.SetActive(false); 
        panPartie.SetActive(false);
        panReglages.SetActive(true);

        //Utilisation des sprites nécéssaires
        btnPartie.GetComponent<Image>().sprite = spritePartie_neutre;
        btnProfil.GetComponent<Image>().sprite = spriteProfil_neutre;
        btnReglages.GetComponent<Image>().sprite = spriteReglages_actif;

    }

}

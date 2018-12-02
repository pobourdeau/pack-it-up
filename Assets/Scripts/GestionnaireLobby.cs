﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GestionnaireLobby : MonoBehaviour {

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

    public Toggle cchKnight;
    public Toggle cchMage;

    public Image imgPerso;
    public Sprite sprite_knight;
    public Sprite sprite_mage;

    public static string choixPersoJoueur = "knight";


    // Use this for initialization
    void Start () {
        btnPartie.GetComponent<Button>().onClick.AddListener(() => AfficherPartie());
        btnProfil.GetComponent<Button>().onClick.AddListener(() => AfficherProfil());
        btnReglages.GetComponent<Button>().onClick.AddListener(() => AfficherReglages());

        cchKnight.onValueChanged.AddListener(delegate {
            if (cchKnight.isOn) {
                cchMage.isOn = false;
                imgPerso.sprite = sprite_knight;
                choixPersoJoueur = "knight";
            }
            else {
                cchMage.isOn = true;
                imgPerso.sprite = sprite_mage;
                choixPersoJoueur = "mage";
            }
        });

        cchMage.onValueChanged.AddListener(delegate {
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

}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Gérer les déplacements du joueur, attaques, dégâts(vie) et les interractions avec les ressources (inventaire et assemblage de l'arme)
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-11-12
 */

public class DeplacementPerso : MonoBehaviour {

    private Rigidbody rbPerso; // Rigidbody du joueur
    private Animator animPerso; // Animator du joueur

    public float vitesseDeplacement = 10f; // Vitesse de déplacement du joueur
    public float vDeplacement; // Vélocité de déplacement
    public float vRotation; // Vélocité de rotation
    public int indVie = 3; // Le nombre de vie restante

    public GameObject txtConstruireArme; // Texte de construction de l'arme
    public GameObject txtRecolter; // Texte de récolte de ressources
    public GameObject arme; // Arme du joueur
    private bool entrainDeConstruire = false; // S'il est entrain de construire son arme
    private bool aLarme = false; // S'il a l'arme
    public Image imgConstruire; // Image timer de construction
    public GameObject oImgConstruire; // GameObject du timer de construction
    public GameObject camSuivie; // Caméra qui suit le personnage
    public GameObject[] aBarreVie; // Barre de vie
    public Sprite vieVide; // Sprite de coeur vide
    public bool attaque = false; // Si le joueur attaque

    public int[] aInventaire; // Inventaire du joueur
    // aInventaire[0] = Bois, aInventaire[1] = Fer, aInventaire[2] = Cuir
    public GameObject[] aCrochetInv; // Les crochets de l'inventaire 
    // aCrochetInv[0] = crochet Bois, aCrochetInv[1] = crochet Fer, aCrochetInv[2] = crochet Cuir
    public GameObject[] aCaseRougeInv; // Les cases rouges de l'inventaire 
    // aCaseRougeInv[0] = case Bois, aCaseRougeInv[1] = case Fer, aCaseRougeInv[2] = case Cuir


    /**
     * Initialisation des variables
     * @param void
     * @return void
     */
    void Start () {
        // Rigidbody du joueur
        rbPerso = GetComponent<Rigidbody>();
        // Animator du joueur
        animPerso = GetComponent<Animator>();

        // Inventaire du joueur
        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;
	}


    /**
     * Vérifier si le joueur est entrain de construire son arme
     * @param void
     * @return void
     */
    void Update() {

        // Si l'animation d'attaque avec épée joue,
        if (animPerso.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
            // Attaque est à true
            attaque = true;
        }
        else {
            attaque = false;
        }


        // Si le personnage n'est pas mort,
        if (animPerso.GetBool("mort") == false) {
                    
            // Si le joueur est entrain de construire son arme,
            if (entrainDeConstruire) {
                // Jouer l'animation de construction
                AnimationConstruireArme();
            }
            else {
                // Arrêter le timer de construction
                StopCoroutine("ConstructionArme");
            }

            // Gestion du mouvement du blend tree
		    animPerso.SetFloat("VelY",Input.GetAxis("Vertical"));
            animPerso.SetFloat("VelX",Input.GetAxis("Horizontal"));
        
            // Si le joueur appui sur la touche droite de la souris,
            if (Input.GetKeyDown(KeyCode.Mouse0) && aLarme) {
                // Faire jouer l'animation d'attaque    
                animPerso.SetTrigger("attaque");
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && aLarme == false) {
                animPerso.SetTrigger("attaque");
            }

            // Gestion de la vie du personnage
            GestionVie();
        }
        else {
            StartCoroutine("OuvrirMenu");
        }
    }


    /**
     * Gérer le déplacement du joueur
     * @param void
     * @return void
     */
    void FixedUpdate() {
        // Si le personnage n'est pas mort,
        if (animPerso.GetBool("mort") == false) {
            // Déplacer le personnage
            transform.Rotate(0, Input.GetAxis("Horizontal") * vRotation, 0);

            vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;

            rbPerso.velocity = (transform.forward * vDeplacement) + new Vector3(0, rbPerso.velocity.y, 0);
        }
    }


    /**
     * Lorsqu'on entre dans la zone de collision avec un collider
     * @param Collider objCollider
     * @return void
     */
    void OnTriggerEnter(Collider objCollider) {

        // Si le joueur n'a pas d'arme
        if (aLarme == false) {
            // Tag de l'objCollider
            switch (objCollider.gameObject.tag) {
                // Bois
                case "bois":
                    // Si le joueur n'a pas de bois dans son inventaire,
                    if (aInventaire[0] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Fer
                case "fer":
                    // Si le joueur n'a pas de fer dans son inventaire,
                    if (aInventaire[1] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Cuir
                case "cuir":
                    // Si le joueur n'a pas de cuir dans son inventaire,
                    if (aInventaire[2] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Forge
                case "forge":
                    // Si le joueur a récolté toutes les ressources
                    if(aInventaire[0] >= 1 && aInventaire[1] >= 1 && aInventaire[2] >= 1) {
                        // Afficher le texte de construction
                        txtConstruireArme.SetActive(true);
                    }
                    else {
                        // Désactiver le texte de récolte
                        txtConstruireArme.SetActive(false);
                    }

                    // Faire clignoter les case vides de l'inventaire
                    if(aInventaire[0] < 1) {
                        aCaseRougeInv[0].SetActive(true);
                        aCaseRougeInv[0].GetComponent<Animator>().enabled = true;
                    }
                    if(aInventaire[1] < 1) {
                        aCaseRougeInv[1].SetActive(true);
                        aCaseRougeInv[1].GetComponent<Animator>().enabled = true;
                    }
                    if (aInventaire[2] < 1) {
                        aCaseRougeInv[2].SetActive(true);
                        aCaseRougeInv[2].GetComponent<Animator>().enabled = true;
                    }
                    break;
                // Arme
                case "arme":
                    animPerso.SetTrigger("dommage");
                    indVie--;
                    break;
            }
        }

        // Si on rentre dans la maison,
        if (objCollider.gameObject.name == "maison") {
            // Changer la position de la caméra
            camSuivie.GetComponent<DeplacementCam>().distanceCamera = new Vector3(0, 15f, 8f);
        }
    }


    /**
     * Lorsqu'on reste dans la zone de collision avec un collider
     * @param Collider objCollider
     * @return void
     */
    private void OnTriggerStay(Collider objCollider) {
        // Si on appuie sur la touche Gauche de la souris et que le joueur n'a pas construit son arme,
        if (Input.GetMouseButtonDown(0) && aLarme == false) {

            // Si l'objet toucher est le bois et que le joueur n'en dispose pas dans son inventaire,
            if(objCollider.gameObject.tag == "bois" && aInventaire[0] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[0] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[0].SetActive(true);
            }

            // Si l'objet toucher est le fer et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "fer" && aInventaire[1] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[1] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[1].SetActive(true);
            }

            // Si l'objet toucher est le cuir et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "cuir" && aInventaire[2] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[2] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                objCollider.gameObject.SetActive(false);
                txtRecolter.SetActive(false);
                aCrochetInv[2].SetActive(true);
            }

            // Si l'objet toucher est le bois et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "forge") {
                // Si le joueur possède tous les objets dans son inventaire,
                if (aInventaire[0] >= 1 && aInventaire[1] >= 1 && aInventaire[2] >= 1) {
                    // Entrain de construire
                    entrainDeConstruire = true;

                    // Afficher le timer de construction
                    oImgConstruire.SetActive(true);
                    imgConstruire.fillAmount = 1;
                    txtConstruireArme.SetActive(false);
                    StartCoroutine("ConstructionArme"); 
                }
            }

        }
    }


    /**
     * À la sortie de la collision avec un collider
     * @param Collider objCollider
     * @return void
     */
    void OnTriggerExit(Collider objCollider) {

        // Si on quitte le collider du bois, du fer et du cuir,
        if(objCollider.gameObject.tag == "bois" || objCollider.gameObject.tag == "fer" || objCollider.gameObject.tag == "cuir") {
            txtRecolter.SetActive(false);
        }
        
        // Si on quitte le collider de la forge,
        if(objCollider.gameObject.tag == "forge") {
            // Entrain de constuire est à faux
            entrainDeConstruire = false;
            // Désactiver l'image du timer de construction et le texte
            oImgConstruire.SetActive(false);
            txtConstruireArme.SetActive(false);

            // Désactiver le clignotement les case vides de l'inventaire
            aCaseRougeInv[0].GetComponent<Animator>().enabled = false;
            aCaseRougeInv[1].GetComponent<Animator>().enabled = false;
            aCaseRougeInv[2].GetComponent<Animator>().enabled = false;

            aCaseRougeInv[0].SetActive(false);
            aCaseRougeInv[1].SetActive(false);
            aCaseRougeInv[2].SetActive(false);
        }

        // Si le personnage rentre dans la maison,
        if (objCollider.gameObject.name == "maison") {
            // Changer la position de la caméra
            camSuivie.GetComponent<DeplacementCam>().distanceCamera = new Vector3(0, 20f, 10f);
        }
    }

    /**
     * Construire l'arme
     * @param void
     * @return void
     */
    private IEnumerator ConstructionArme() {
        // Attendre que l'arme se construise
        yield return new WaitForSeconds(5f);
        oImgConstruire.SetActive(false);

        // Retirer les ressources de l'inventaire du joueur
        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;

        print("Bravo");

        // Afficher l'arme et 
        aLarme = true;
        arme.SetActive(true);
        animPerso.SetBool("aLarme", true);
    }

    /**
     * Animation d'un timer quand on construit l'arme
     * @param void
     * @return void
     */
    private void AnimationConstruireArme() {
        imgConstruire.fillAmount -= 1.0f / 5f * Time.deltaTime;
    }

    /**
     * Gérer l'affichage de l'état de la vie du personnage
     * @param void
     * @return void
     */
    private void GestionVie() {

        // La vie du personnage
        switch (indVie) {
            case 2:
                aBarreVie[2].GetComponent<Image>().sprite = vieVide;
                break;
            case 1:
                aBarreVie[1].GetComponent<Image>().sprite = vieVide;
                break;
            case 0:
                aBarreVie[0].GetComponent<Image>().sprite = vieVide;

                // Jouer l'animation de mort
                animPerso.SetBool("mort", true);
                break;

        }
    }

    /**
     * Attendre 3 secondes avant de retourner au menu principal
     * @param void
     * @return void
     */
    public IEnumerator OuvrirMenu() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SceneMenu");
    }
}

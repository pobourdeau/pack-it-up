using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Gérer les déplacements du joueur et les interractions avec les ressources (inventaire et assemblage de l'arme)
 * @author Pier-Olivier Bourdeau
 * @version 2018-10-29
 */

public class DeplacementPerso : MonoBehaviour {

    private Rigidbody rbPerso; // Rigidbody du joueur
    private Animator animPerso; // Animator du joueur

    public float vitesseDeplacement = 10f; // Vitesse de déplacement du joueur
    public float vDeplacement; // Vélocité de déplacement
    public float vRotation; // Vélocité de rotation

    public GameObject txtConstruireArme; // Texte de construction de l'arme
    public GameObject txtRecolter; // Texte de récolte de ressources
    public GameObject arme; // Arme du joueur
    private bool entrainDeConstruire = false; // S'il est entrain de construire son arme
    private bool aLarme = false; // S'il a l'arme
    public Image imgConstruire; // Image timer de construction
    public GameObject oImgConstruire; // GameObject du timer de construction

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

        // Si le joueur est entrain de construire son arme,
        if (entrainDeConstruire) {
            // Jouer l'animation de construction
            AnimationConstruireArme();
        }
        else {
            // Arrêter le timer de construction
            StopCoroutine("ConstructionArme");
        }
    }


    /**
     * Gérer le déplacement du joueur
     * @param void
     * @return void
     */
    void FixedUpdate() {
        //
        transform.Rotate(0, Input.GetAxis("Horizontal") * vRotation, 0);

        vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;

        rbPerso.velocity = (transform.forward * vDeplacement) + new Vector3(0, rbPerso.velocity.y, 0);   
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
            }
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
    }

    /**
     * Construire l'arme
     * @param void
     * @return void
     */
    private IEnumerator ConstructionArme() {
        yield return new WaitForSeconds(5f);
        oImgConstruire.SetActive(false);

        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;

        aLarme = true;
        arme.SetActive(true);
    }

    /**
     * Animation d'un timer quand on construit l'arme
     * @param void
     * @return void
     */
    private void AnimationConstruireArme() {
        imgConstruire.fillAmount -= 1.0f / 5f * Time.deltaTime;
    }
}

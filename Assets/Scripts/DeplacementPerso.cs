﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

/**
 * Gérer les déplacements du joueur, attaques, dégâts(vie) et les interractions avec les ressources (inventaire et assemblage de l'arme)
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-11-12
 */

public class DeplacementPerso : MonoBehaviourPunCallbacks, IPunObservable {

    // Les sons
    public AudioClip bruitSlash;
    public AudioClip bruitAttacked;
    public AudioClip bruitDead;
    public AudioClip bruitAttackSpecial;
    public AudioClip bruitForge;

    public static GameObject LocalPlayerInstance;

    private Rigidbody rbPerso; // Rigidbody du joueur
    private Animator animPerso; // Animator du joueur
    private AudioSource audioSourcePerso;
    private float LongueurRayCast = 100f; // Distance maximale du RayCast

    public float vitesseDeplacement = 10f; // Vitesse de déplacement du joueur
    public float vDeplacement; // Vélocité de déplacement
    public float vRotation; // Vélocité de rotation
    public int indVie = 3; // Le nombre de vie restante

    public GameObject txtConstruireArme; // Texte de construction de l'arme
    public GameObject txtRecolter; // Texte de récolte de ressources
    public GameObject arme; // Arme du joueur
    private Renderer brasRenderer; // renderer du bras
    private Renderer corpsRenderer; // renderer du corps
    
    private bool entrainDeConstruire = false; // S'il est entrain de construire son arme
    private bool aLarme = false; // S'il a l'arme
    private bool stunned = false; //Si le joueur s'est récemment fait frappé
    private bool mainEpee = false; //Determiner si le joueur est frapper par une main 
    public Image imgConstruire; // Image timer de construction
    public GameObject oImgConstruire; // GameObject du timer de construction
    public GameObject[] aBarreVie; // Barre de vie
    public Sprite vieVide; // Sprite de coeur vide
    public bool attaque = false; // Si le joueur attaque
    public GameObject oMain;

    public int[] aInventaire; // Inventaire du joueur
    // aInventaire[0] = Bois, aInventaire[1] = Fer, aInventaire[2] = Cuir
    public GameObject[] aCrochetInv; // Les crochets de l'inventaire 
    // aCrochetInv[0] = crochet Bois, aCrochetInv[1] = crochet Fer, aCrochetInv[2] = crochet Cuir
    public GameObject[] aCaseRougeInv; // Les cases rouges de l'inventaire 
    // aCaseRougeInv[0] = case Bois, aCaseRougeInv[1] = case Fer, aCaseRougeInv[2] = case Cuir

    

    
    void Awake() {
        if (photonView.IsMine) {
            DeplacementPerso.LocalPlayerInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
    }


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
        // Speaker du joueur
        audioSourcePerso = GetComponent<AudioSource>();
        //Chercher le renderer du bras et du corps
        brasRenderer = GameObject.Find("perso/Main").GetComponent<SkinnedMeshRenderer>();
        corpsRenderer = GameObject.Find("perso/Corps").GetComponent<SkinnedMeshRenderer>();
        
        // Inventaire du joueur
        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;


        DeplacementCam _cameraWork = this.gameObject.GetComponent<DeplacementCam>();

        if (_cameraWork != null) {
            if (photonView.IsMine) {
                _cameraWork.OnStartFollowing();
            }
        }
        else {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }


    /**
     * Vérifier si le joueur est entrain de construire son arme
     * @param void
     * @return void
     * @author Vincent Gagnon
     */
    void Update() {

        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }


        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit infoCollision;

        // Si le tir part, créer un nouveau raycast qui part du fusil
        if (Physics.Raycast(camRay.origin, camRay.direction, out infoCollision, 5000, LayerMask.GetMask("Plancher"))) {
            Vector3 pointARegarder = infoCollision.point;

            pointARegarder.y = 0;
            transform.LookAt(pointARegarder);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }


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

            // Si le joueur appui sur la touche droite de la souris,
            if (photonView.IsMine) {
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    // Faire jouer l'animation d'attaque
                    if(animPerso.GetCurrentAnimatorStateInfo(0).IsName("normal") && stunned==false){
                        audioSourcePerso.PlayOneShot(bruitSlash, 0.7F);    
                        animPerso.SetTrigger("attaque");
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    
                    if(stunned==false && aLarme){
                        attaqueSpecial();
                    }
                    
                }
            }
            

            // Gestion de la vie du personnage
            //GestionVie();
        }
        else {
            StartCoroutine("OuvrirMenu");
        }
    }


    /**
     * Gérer le déplacement du joueur
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    void FixedUpdate() {

        // Si le personnage n'est pas mort,
        /* 
        if (animPerso.GetBool("mort") == false) {
             
            // Si le personnage n'est pas mort,
            if (animPerso.GetBool("mort") == false) {
                // Déplacer le personnage
                transform.Rotate(0, Input.GetAxis("Horizontal") * vRotation, 0);

                vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;

                rbPerso.velocity = (transform.forward * vDeplacement) + new Vector3(0, rbPerso.velocity.y, 0);
            }
            */
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                return;
            } 
            var vDeplacement = Input.GetAxis("Vertical");
            //var hDeplacement = Input.GetAxis("Horizontal");
            
            rbPerso.velocity = transform.forward * vDeplacement * vitesseDeplacement;

            // Gestion du mouvement du blend tree
            animPerso.SetFloat("VelY", vDeplacement);
            //animPerso.SetFloat("VelX", hDeplacement);

            // Faire pivoter le joueur
            Pivoter();
        }


    


    /**
     * Lorsqu'on entre dans la zone de collision avec un collider
     * @param Collider objCollider
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    void OnTriggerEnter(Collider objCollider) {
        
        // Si le joueur n'a pas d'arme
        if (aLarme == false) {
            // Tag de l'objCollider
            switch (objCollider.gameObject.tag) {
                // Bois
                case "bois":
                    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                        return;
                    }
                    // Si le joueur n'a pas de bois dans son inventaire,
                    if (aInventaire[0] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Fer
                case "fer":
                    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                        return;
                    }
                    // Si le joueur n'a pas de fer dans son inventaire,
                    if (aInventaire[1] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Cuir
                case "cuir":
                    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                        return;
                    }
                    // Si le joueur n'a pas de cuir dans son inventaire,
                    if (aInventaire[2] < 1) {
                        // Afficher le texte de récolte
                        txtRecolter.SetActive(true);
                    }
                    break;
                // Forge
                case "forge":
                    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                        return;
                    }
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
                    //Si le personnage attaqué est celui du joueur finir la fonction, sinon jouer la fonction de gestion de vie
                    

                    rbPerso.AddForce(objCollider.transform.forward * 10f);

                    mainEpee=false;    
                    GestionVie(mainEpee);
                    
                    break;
                case "main":
                    //Si le personnage attaqué est celui du joueur finir la fonction, sinon jouer la fonction de gestion de vie
                    

                     mainEpee=true;    
                    GestionVie(mainEpee);
                break;    
            }
        }

        // Si on rentre dans la maison,
        if (objCollider.gameObject.name == "maison") {
            // Changer la position de la caméra
        }
    }


    /**
     * Lorsqu'on reste dans la zone de collision avec un collider
     * @param Collider objCollider
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    private void OnTriggerStay(Collider objCollider) {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        // Si on appuie sur la touche Gauche de la souris et que le joueur n'a pas construit son arme,
        if (Input.GetKeyDown(KeyCode.E) && aLarme == false) {
            
            // Si l'objet toucher est le bois et que le joueur n'en dispose pas dans son inventaire,
            if(objCollider.gameObject.tag == "bois" && aInventaire[0] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[0] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                //objCollider.gameObject.SetActive(false);
                PhotonNetwork.Destroy(objCollider.gameObject);
                txtRecolter.SetActive(false);
                aCrochetInv[0].SetActive(true);
            }

            // Si l'objet toucher est le fer et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "fer" && aInventaire[1] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[1] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                //objCollider.gameObject.SetActive(false);
                PhotonNetwork.Destroy(objCollider.gameObject);
                txtRecolter.SetActive(false);
                aCrochetInv[1].SetActive(true);
            }

            // Si l'objet toucher est le cuir et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "cuir" && aInventaire[2] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[2] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                //objCollider.gameObject.SetActive(false);
                PhotonNetwork.Destroy(objCollider.gameObject);
                txtRecolter.SetActive(false);
                aCrochetInv[2].SetActive(true);
            }

            // Si l'objet toucher est le bois et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "forge") {
                // Si le joueur possède tous les objets dans son inventaire,
                if (aInventaire[0] >= 1 && aInventaire[1] >= 1 && aInventaire[2] >= 1) {
                    // Entrain de construire
                    entrainDeConstruire = true;
                    
                    //Déclencher le son de la forge
                    audioSourcePerso.PlayOneShot(bruitForge, 0.7F);

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
     * @author Pier-Olivier Bourdeau
     */
    void OnTriggerExit(Collider objCollider) {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        // Si on quitte le collider du bois, du fer et du cuir,
        if(objCollider.gameObject.tag == "bois" || objCollider.gameObject.tag == "fer" || objCollider.gameObject.tag == "cuir") {
            txtRecolter.SetActive(false);
        }
        
        // Si on quitte le collider de la forge,
        if(objCollider.gameObject.tag == "forge") {
            //Arrêter le bruit de la forge
            //audioSourcePerso.Stop();

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
        }
    }

    /**
     * Construire l'arme
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    private IEnumerator ConstructionArme() {
 
        // Attendre que l'arme se construise
        yield return new WaitForSeconds(5f);
        oImgConstruire.SetActive(false);

        // Retirer les ressources de l'inventaire du joueur
        aInventaire[0] = 0;
        aInventaire[1] = 0;
        aInventaire[2] = 0;

        //Detruire la hitbox de la main
        //PhotonNetwork.Destroy(GameObject.Find("perso/Main/hitboxMain"));
        // Créer l'arme et l'a donner au joueur
        GameObject oArme;

        // Si le personnage est le knight,
        if (this.gameObject.tag == "knight") {
            // Créer et donner une épée comme arme
            oArme = PhotonNetwork.Instantiate("epee", oMain.transform.position, oMain.transform.rotation * Quaternion.Euler(new Vector3(74.65f, -12.11f, 0f)));
        }
        else {
            // Créer et donner un staff comme arme
            oArme = PhotonNetwork.Instantiate("staff", oMain.transform.position, oMain.transform.rotation * Quaternion.Euler(new Vector3(75f, -120f, 0f)));
        }
        
        oArme.transform.parent = oMain.transform;
        aLarme = true;
        animPerso.SetBool("aLarme",true);
    }

    /**
     * Animation d'un timer quand on construit l'arme
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    private void AnimationConstruireArme() {
        imgConstruire.fillAmount -= 1.0f / 5f * Time.deltaTime;
    }

    /**
     * Gérer l'affichage de l'état de la vie du personnage
     * @param void
     * @return void
     * @author Issam Aloulou
     */
    private void GestionVie(bool main) {
        // La vie du personnage
        if(stunned==false){
            animPerso.SetTrigger("dommage");
            if(main==false){
                indVie--;
            }
            else{
                Debug.Log(main);
                StartCoroutine("Invulnerable");
                vitesseDeplacement = 10f;
                return;
            }
            
            switch (indVie) {
                case 2:
                    aBarreVie[2].GetComponent<Image>().sprite = vieVide;
                    StartCoroutine("Invulnerable");
                    audioSourcePerso.PlayOneShot(bruitAttacked,1f);
                    break;
                case 1:
                    aBarreVie[1].GetComponent<Image>().sprite = vieVide;
                    StartCoroutine("Invulnerable");
                    audioSourcePerso.PlayOneShot(bruitAttacked,1f);
                    break;
                case 0:
                    aBarreVie[0].GetComponent<Image>().sprite = vieVide;
                    audioSourcePerso.PlayOneShot(bruitDead,1f);

                    // Jouer l'animation de mort
                    animPerso.SetBool("mort", true);

                    SceneManager.LoadScene("SceneMenu");
                    break;
            }
        }
        return;
    }
    /**
     * Si le personnage se fait attaquer, le faire clignoter et l'empecher d'attaquer ou de se faire attaquer
     * @param arme
     * @return void
     * Auteur: Issam Aloulou  
     */
    public IEnumerator Invulnerable(){
        stunned=true;
        for(int i = 0; i<3 ; i++){
        brasRenderer.enabled= false;
        corpsRenderer.enabled= false;
        yield return new WaitForSeconds(0.3f);
        brasRenderer.enabled= true;
        corpsRenderer.enabled= true;
        yield return new WaitForSeconds(0.3f);
        }
        stunned=false;
        vitesseDeplacement = 16f;
    }
    
    /**
     * Attendre 3 secondes avant de retourner au menu principal
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public IEnumerator OuvrirMenu() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SceneMenu");
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(attaque);
            stream.SendNext(indVie);
        }
        else {
            // Network player, receive data
            this.attaque = (bool)stream.ReceiveNext();
            this.indVie = (int)stream.ReceiveNext();
        }
    }


    /**
     * Faire pivoter le personnage en fonction de la souris du joueur
     * @param void
     * @return void;
     * Auteur: Issam Aloulou
     */
    void Pivoter() {

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, LongueurRayCast)) {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 pointARegarder = hit.point - transform.position;

            pointARegarder.y = 0f;

            // Regarder le nouveau point
            Quaternion rotation = Quaternion.LookRotation(pointARegarder);

            // Faire pivoter le joueur
            rbPerso.MoveRotation(rotation);
        }
    }

    /**
     * Identifier le personnage choisi pour effectuer l'attaque special de celui-ci
     * @param void
     * @return void;
     * Auteur: Issam Aloulou
     */
    void attaqueSpecial() {

        if(name == "knight(Clone)"){
            StartCoroutine("Sprint");
        }
        else{
            BouleDeFeu();
        }
    }

    /**
     * 
     * @param void
     * @return void;
     * Auteur: Issam Aloulou
     */
    public IEnumerator Sprint(){
        vitesseDeplacement = 25f;
        yield return new WaitForSeconds(0.3f);
        if(stunned==false) vitesseDeplacement = 16f;
    }

    /**
     * 
     * @param void
     * @return void;
     * Auteur: Issam Aloulou
     */
    void BouleDeFeu(){

    }

}

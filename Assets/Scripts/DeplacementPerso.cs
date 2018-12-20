using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/**
 * Gérer les déplacements du joueur, attaques, dégâts(vie) et les interractions avec les ressources (inventaire et assemblage de l'arme)
 * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou
 * @version 2018-12-18
 */
public class DeplacementPerso : MonoBehaviourPunCallbacks, IPunObservable {

    public static GameObject LocalPlayerInstance;

    // Les sons
    public AudioClip bruitSlash; // Son de slash
    public AudioClip bruitAttacked; // Son d'attaqué
    public AudioClip bruitDead; // Son de mort
    public AudioClip bruitAttackSpecial; // Son d'attaque spéciale
    public AudioClip bruitForge; // Son de la forge


    private Rigidbody rbPerso; // Rigidbody du joueur
    private Animator animPerso; // Animator du joueur
    private AudioSource audioSourcePerso; // AudioSource du joueur
    private float LongueurRayCast = 100f; // Distance maximale du RayCast

    public float vitesseDeplacement = 10f; // Vitesse de déplacement du joueur
    public float vDeplacement; // Vélocité de déplacement
    public float vRotation; // Vélocité de rotation
    public int indVie = 3; // Le nombre de vie restante

    public GameObject txtConstruireArme; // Texte de construction de l'arme
    public GameObject txtRecolter; // Texte de récolte de ressources
    public GameObject arme; // Arme du joueur

    private GameObject corps;
    
    private bool entrainDeConstruire = false; // S'il est entrain de construire son arme
    private bool aLarme = false; // S'il a l'arme
    private bool stunned = false; //Si le joueur s'est récemment fait frappé
    public Image imgConstruire; // Image timer de construction
    public GameObject oImgConstruire; // GameObject du timer de construction
    public GameObject[] aBarreVie; // Barre de vie
    public Sprite vieVide; // Sprite de coeur vide
    public bool attaque = false; // Si le joueur attaque
    public GameObject oMain; // La main du joueur

    public int[] aInventaire; // Inventaire du joueur
    public GameObject oInventaire; // Objet de l'inventaire du joueur
    public GameObject txtSpectateur; // Texte qui dit si le joueur devient un spectateur
    public GameObject[] aCrochetInv; // Les crochets de l'inventaire 
    public GameObject[] aCaseRougeInv; // Les cases rouges de l'inventaire 

    public GameObject firePoint; //Endroit d'ou le projectile est tiré
    private float timeToFire = 0;
    public GameObject GuerrierTrail;

    /**
     * Déterminer si c'est le joueur de l'ordinateur client
     * @param void
     * @return void
     */
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
    void Start() {
        // Rigidbody du joueur
        rbPerso = GetComponent<Rigidbody>();

        // Animator du joueur
        animPerso = GetComponent<Animator>();

        // Speaker du joueur
        audioSourcePerso = GetComponent<AudioSource>();

        //Chercher le renderer du bras et du corps
        corps = transform.Find("perso").gameObject;

        // Inventaire du joueur
        aInventaire[0] = 1;
        aInventaire[1] = 1;
        aInventaire[2] = 1;

        // Faire en sorte que la caméra suit le joueur de l'ordinateur client
        DeplacementCam _cameraWork = this.gameObject.GetComponent<DeplacementCam>();

        if (_cameraWork != null) {
            if (photonView.IsMine) {
                _cameraWork.OnStartFollowing();
            }
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

            if (photonView.IsMine) {
                // Si le joueur appui sur la touche gauche de la souris,
               if (Input.GetKeyDown(KeyCode.Mouse0)) {
       
                    // Faire jouer l'animation d'attaque
                    if (animPerso.GetCurrentAnimatorStateInfo(0).IsName("normal") && stunned == false) {
                        audioSourcePerso.PlayOneShot(bruitSlash, 0.7F);
                        StartCoroutine("hitboxAttaque");
                        animPerso.SetTrigger("attaque");

                        // Si le joueur à une arme
                        if (aLarme) {
                            // Permettre d'attaquer
                            StartCoroutine("Attaque", "arme");
                        }
                        else {
                            StartCoroutine("Attaque", "main");
                        }
                        
                    }
                }

                // Si le joueur appui sur la touche droite de la souris
                if (Input.GetKeyDown(KeyCode.Mouse1)) {

                    if (stunned == false && aLarme && Time.time >= timeToFire) {
                        attaqueSpecial();
                        //WaitDude();
                    }
                }               
            }
        }
    }

    /**
     * Créer un hitbox devant le joueur pour émettre des dégats aux autres joueurs
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public IEnumerator Attaque(string sTypeAttaque) {
        // Attendre 0.3 sec
        yield return new WaitForSeconds(0.3f);

        // Créer un objet devant le joueur et le parainer
        GameObject goHitBox;

        if (sTypeAttaque == "arme") {
            goHitBox = PhotonNetwork.Instantiate("HitBoxArme", transform.position + transform.forward * 2.5f, gameObject.transform.rotation);
        }
        else {
            goHitBox = PhotonNetwork.Instantiate("HitBoxMain", transform.position + transform.forward * 2.5f, gameObject.transform.rotation);
        }

        goHitBox.transform.parent = this.gameObject.transform;

        // Attendre 0.2 sec
        yield return new WaitForSeconds(2f);

        // Détruire l'objet
        PhotonNetwork.Destroy(goHitBox);
    }

    /**
     * Gérer le déplacement du joueur
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    void FixedUpdate() {

        // Si le personnage n'est pas mort,
        if (animPerso.GetBool("mort") == false) {

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
                return;
            }

            // Gestion des touches du clavier
            var vDeplacement = Input.GetAxis("Vertical");
            
            // Déplacer le joueur vers l'avant ou l'arrière
            rbPerso.velocity = transform.forward * vDeplacement * vitesseDeplacement;

            // Gestion du mouvement du blend tree
            animPerso.SetFloat("VelY", vDeplacement);

            // Faire pivoter le joueur
            Pivoter();
        }
    }


    /**
     * Si l'arme rentre en collision avec le personnage
     * @param Collision collision
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void OnCollisionEnter(Collision collision) {
        // Si on se fait toucher par une arme,
        if(collision.gameObject.tag == "arme") {
            // Descendre la vie
            GestionVie(false);
        }

        // Si on se fait toucher par un spell,
        if (collision.gameObject.tag == "spell") {
            // Descendre la vie
            GestionVie(false);
        }

        if(collision.gameObject.tag == "main") {
            GestionVie(true);
        }
    }
    


    /**
     * Lorsqu'on entre dans la zone de collision avec un collider
     * @param Collider objCollider
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void OnTriggerEnter(Collider objCollider) {
        
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
                case "maison":
                    // Changer la position de la caméra
                    GetComponent<DeplacementCam>().distance = 10f;
                    GetComponent<DeplacementCam>().height = 10f;
                    break;
            }
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
                int pvID = objCollider.gameObject.GetComponent<PhotonView>().ViewID;
                photonView.RPC("DetruireObjet", RpcTarget.MasterClient, pvID);

                txtRecolter.SetActive(false);
                aCrochetInv[0].SetActive(true);
            }

            // Si l'objet toucher est le fer et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "fer" && aInventaire[1] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[1] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                int pvID = objCollider.gameObject.GetComponent<PhotonView>().ViewID;
                photonView.RPC("DetruireObjet", RpcTarget.MasterClient, pvID);

                txtRecolter.SetActive(false);
                aCrochetInv[1].SetActive(true);
            }

            // Si l'objet toucher est le cuir et que le joueur n'en dispose pas dans son inventaire,
            if (objCollider.gameObject.tag == "cuir" && aInventaire[2] < 1) {
                // L'ajouter dans son inventaire
                aInventaire[2] = 1;

                // Cacher le morceau de bois pour ne pas le réutiliser et l'afficher dans l'inventaire
                int pvID = objCollider.gameObject.GetComponent<PhotonView>().ViewID;
                photonView.RPC("DetruireObjet", RpcTarget.MasterClient, pvID);

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
     * Détruire un objet sur tous les ordinateurs en même temps
     * @param int pvID
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    [PunRPC]
    public void DetruireObjet(int pvID) {
        PhotonNetwork.Destroy(PhotonView.Find(pvID));
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
            GetComponent<DeplacementCam>().distance = 12.66f;
            GetComponent<DeplacementCam>().height = 16f;
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
        // Créer l'arme et l'a donner au joueu
        // Si le personnage est le knight,
        if (this.gameObject.tag == "knight") {
            // Créer et donner une épée comme arme
            arme = PhotonNetwork.Instantiate("epee", oMain.transform.position, oMain.transform.rotation * Quaternion.Euler(new Vector3(74.65f, -12.11f, 0f)));
        }
        else {
            // Créer et donner un staff comme arme
            arme = PhotonNetwork.Instantiate("staff", oMain.transform.position, oMain.transform.rotation * Quaternion.Euler(new Vector3(75f, -120f, 0f)));
        }

        arme.transform.parent = oMain.transform;
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

        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }

        // La vie du personnage
        if (stunned==false){
            animPerso.SetTrigger("dommage");

            if(main == false){
                indVie--;
            }
            else{
                StartCoroutine("Invulnerable");
                vitesseDeplacement = 10f;
                return;
            }

            // Gestion de la vie du personnage
            switch (indVie) {
                case 2:
                    // Enlever un coeur et faire jouer un son
                    aBarreVie[2].GetComponent<Image>().sprite = vieVide;
                    StartCoroutine("Invulnerable");
                    audioSourcePerso.PlayOneShot(bruitAttacked,1f);
                    break;
                case 1:
                    // Enlever deux coeurs et faire jouer un son
                    aBarreVie[1].GetComponent<Image>().sprite = vieVide;
                    StartCoroutine("Invulnerable");
                    audioSourcePerso.PlayOneShot(bruitAttacked,1f);
                    break;
                case 0:
                    // Enlever trois coeurs et faire jouer un son
                    aBarreVie[0].GetComponent<Image>().sprite = vieVide;
                    audioSourcePerso.PlayOneShot(bruitDead,1f);

                    // Jouer l'animation de mort
                    animPerso.SetBool("mort", true);

                    // Faire mourir le personnage
                    Mourir();
                    break;
            }

            // Mettre à jour le nombre de vie du joueur pour tout le monde
            if (photonView.IsMine) {
                object lives;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerLives", out lives)) {
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "PlayerLives", ((int)lives <= 1) ? 0 : ((int)lives - 1) } });
                }
            }
        }
    }
    /**
     * Si le personnage se fait attaquer, le faire clignoter et l'empecher d'attaquer ou de se faire attaquer
     * @param arme
     * @return void
     * Auteur: Issam Aloulou  
     */
    public void Invulnerable(GameObject corps){
        stunned=true;
        photonView.RPC("FlasherPersoPartout", RpcTarget.AllViaServer);
    }

    public IEnumerator FlasherPerso(GameObject go) {
        for(int i=0; i<3; i++) {
            go.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            go.SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }

        stunned = false;
        vitesseDeplacement = 16f;
    }

    /**
     * Faire flasher le personnage sur tous les ordinateurs
     * @param bool bEtat, GameObject go
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    [PunRPC]
    public void FlasherPersoPartout() {
        StartCoroutine("FlasherPerso", corps);
    }

    /**
     * Faire mourir le joueur
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    public void Mourir() {
        // Arrêter de suivre le personnage et positionner la caméra dans l'action
        GetComponent<DeplacementCam>().isFollowing = false;
        GetComponent<DeplacementCam>().PositionnerCameraMort();

        // Enlever les coeurs de l'écran
        aBarreVie[2].SetActive(false);
        aBarreVie[1].SetActive(false);
        aBarreVie[0].SetActive(false);
        oInventaire.SetActive(false);

        // Faire disparaitre le joueur sur tous les ordinateurs
        photonView.RPC("DisparaitreJoueur", RpcTarget.AllViaServer);

        // Activer le mode spectateur
        txtSpectateur.SetActive(true);
    }

    /**
     * Faire disparaitre le joueur sur tous les ordinateurs
     * @param void
     * @return void
     * @author Pier-Olivier Bourdeau
     */
    [PunRPC]
    public void DisparaitreJoueur() {
        gameObject.SetActive(false);
    }


    /**
     * Rentre les mouvements des personnages le plus fluide possible
     * @param PhotonStream stream, PhotonMessageInfo info
     * @return void
     * @author Pier-Olivier Bourdeau, Exit Games
     */
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

        if(this.gameObject.tag == "knight") {
            timeToFire = Time.time + 3;
            //StartCoroutine("WaitWarrior");
            StartCoroutine("Sprint");
        }
        else{
            timeToFire = Time.time + 3;
            BouleDeFeu();
        }
    }

    /**
       * @param void
       * @return void;
       * Auteur: Vincent
       */
    public IEnumerator WaitWarrior() {
        yield return new WaitForSeconds(2f);

    }

    /**
     * 
     * @param void
     * @return void;
     * Auteur: Issam Aloulou
     */
    public IEnumerator Sprint(){
        vitesseDeplacement = 50f;
        GuerrierTrail.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        if (stunned==false) vitesseDeplacement = 16f;

        yield return new WaitForSeconds(0.4f);
        GuerrierTrail.SetActive(false);
    }

    /**
     * 
     * @param void
     * @return void;
     * Auteur: Vincent Gagnon
     */
    void BouleDeFeu() {

        if (firePoint != null) {
            GameObject vfx = PhotonNetwork.Instantiate("Fireball", firePoint.transform.position, firePoint.transform.rotation);
            StartCoroutine(WaitDude(vfx, 5f));
        }
    }

   /**
   * 
   * @param void
   * @return void;
   * Auteur: Vincent Gagnon
   */
    public IEnumerator WaitDude(GameObject objectToDestroy, float temps){
        yield return new WaitForSeconds(temps);
        PhotonNetwork.Destroy(objectToDestroy);
    }
}

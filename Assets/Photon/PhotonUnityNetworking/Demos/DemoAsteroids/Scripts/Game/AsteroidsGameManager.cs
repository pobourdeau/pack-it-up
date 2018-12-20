// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsteroidsGameManager.cs" company="Exit Games GmbH">
//   Part of: Asteroid demo
// </copyright>
// <summary>
//  Game Manager for the Asteroid Demo
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Asteroids
{
    /**
     * Gestionnaire du jeu. Gérer les connexions des joueurs, la fin de la partie, le chrono de la partie, l'affichage des menus.
     * @author Pier-Olivier Bourdeau, Vincent Gagnon, Issam Aloulou, Exit Games
     * @version 2018-12-18
     */
    public class AsteroidsGameManager : MonoBehaviourPunCallbacks
    {
        public static AsteroidsGameManager Instance = null;

        // UI
        public Text InfoText; // Info de la partie
        public GameObject panDebut; // UI début de la partie
        public GameObject[] aSpawnPoint; // Tous les spawns point disponible
        public GameObject oHUD; // UI du HUD du joueur
        public GameObject panFin; // UI du menu de fin
        public Text txtGagnant; // UI contenant le nom du joueur gagnant

        // Variables du perso
        public AudioClip[] aBruits; // Array contenant tous les sons
        public AudioClip[] aBruitsMage; // Array contenant tous les sons
        public AudioClip[] aBruitsKnight; // Array contenant tous les sons
        public GameObject txtConstruireArme; // Texte de construction de l'arme
        public GameObject txtRecolter; // Texte de récolte de ressources
        public Image imgConstruire;  // Image timer de construction
        public GameObject oImgConstruire; // GameObject du timer de construction
        public GameObject[] aBarreVie; // Barre de vie
        public Sprite vieVide; // Sprite de coeur vide
        public GameObject[] aCrochetInv; // Les crochets de l'inventaire 
        public GameObject[] aCaseRougeInv; // Les cases rouges de l'inventaire 
        public GameObject oInventaire; // Inventaire du joueur
        public GameObject txtSpectateur; // Texte qui dit si le joueur devient un spectateur

        public GameObject[] aSpawnerBois; // Tous les points de spawn du bois
        public GameObject[] aSpawnerFer; // Tous les points de spawn du fer
        public GameObject[] aSpawnerCuir; // Tous les points de spawn du cuir

        public int[] aTabVieJoueur; // Nombre de vie pour tous les joueurs
        public int iSecondeRestante; // Seconde restante
        public int iMinuteRestante; // Minute restante
        private int iSecondeTotal; // Transformer les minutes en secondes
        public Text txtTimer; // UI text où le temps est écrit
        public Image imgTimer; // Image du timer

        private bool bFin = false; // Est-ce que la partie est finie?

        int nbJoueurConnecte = 0;



        /**
         * Créer une instance du script
         * @param void
         * @return void
         * @author Exit Games
         */
        public void Awake()
        {
            Instance = this;
        }

        /**
         * À l'ouverture de la partie, donner les propriétés
         * @param void
         * @return void
         * @author Exit Games
         */
        public void Start() {
            InfoText.text = "En attentes des autres joueurs...";

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        /**
         * Commencer le timer pour commencer la partie
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        /**
         * Annuler le timer pour commencer la partie
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        /**
         * Afficher le menu de fin de partie avec le gagnant
         * @param void
         * @return String winner, int score
         * @author Exit Games et Pier-Olivier Bourdeau
         */
        private IEnumerator EndOfGame(string winner, int score){

            // Ouvrir le menu de fin
            oHUD.SetActive(false);
            panFin.SetActive(true);

            // Écrire le gagnant de la partie
            txtGagnant.text = "Le gagnant est " + winner;

            // Démarrer un timer pour fermer la partie
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                txtGagnant.text = "Le gagnant est " + winner;

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            // Retourner au menu principal
            PhotonNetwork.LeaveRoom();
        }

        /**
         * À la déconnexion de la partie, retourner à la scène de connexion
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnDisconnected(DisconnectCause cause){
            CheckEndOfGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneRejoindre");
        }


        /**
         * Quand un joueur quitte la partie
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnLeftRoom(){
            CheckEndOfGame();
            PhotonNetwork.Disconnect();
        }


        /**
         * Quand un joueur a quitter la partie
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnPlayerLeftRoom(Player otherPlayer){
            CheckEndOfGame();
        }


        /**
         * Lorsque les propriétés des joueurs changent
         * @param void
         * @return void
         * @author Exit Games
         */
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
            if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    Hashtable props = new Hashtable
                    {
                        {CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time}
                    };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }
            }
        }


        /**
         * Débuter la partie (Créer les personnages, les ressources, le timer, etc)
         * @param void
         * @return void
         * @author Pier-Olivier Bourdeau
         */
        private void StartGame() {

            // Remplir le tableau de vie en fonction du nombre de joueur dans la partie
            int iCompteur = 0;
            foreach (Player p in PhotonNetwork.PlayerList) {
                aTabVieJoueur[iCompteur] = 3;
                iCompteur++;
            }

            // Vérifier s'il y a plus d'un joueur sur la carte
            CheckEndOfGame();

            // Calculer le nombre de seconde total à la partie
            iSecondeTotal = iMinuteRestante * 60 + iSecondeRestante;

            // Appeler la fonction timer à chaque seconde
            InvokeRepeating("Timer", 0, 1f);

            // Créer les ressources
            if (PhotonNetwork.IsMasterClient) {
                GenererRessources();
            }

            // Récupérer l'id du joueur qui se trouve dans le lobby (chaque joueur à un id unique...)
            int idPlayer = (int)PhotonNetwork.LocalPlayer.CustomProperties["idPlayer"];

            // Position et rotation de spawn du joueur
            Vector3 position = aSpawnPoint[idPlayer - 1].transform.position;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, aSpawnPoint[idPlayer - 1].transform.eulerAngles.y, 0));

            // CRÉATION DES PERSONNAGES
            GameObject oClone = PhotonNetwork.Instantiate(PlayerListEntry.nomPerso, position, rotation, 0);
            oClone.GetComponent<DeplacementPerso>().enabled = true;
            oClone.GetComponent<DeplacementCam>().enabled = true;

            // Donner les variables au personnage
            if(PlayerListEntry.nomPerso == "knight") {
                // Bruit dommange du chevalier
                oClone.GetComponent<DeplacementPerso>().bruitAttacked = aBruitsKnight[0];

                // Bruit attaque spéciale du chevalier
                oClone.GetComponent<DeplacementPerso>().bruitAttackSpecial = aBruitsKnight[1];
                oClone.GetComponent<DeplacementPerso>().bruitAttackSpecial2 = aBruitsKnight[2];

                // Bruit de mort du chevalier
                oClone.GetComponent<DeplacementPerso>().bruitDead = aBruitsKnight[3];
            }
            else {
                // Bruit dommange du magicien
                oClone.GetComponent<DeplacementPerso>().bruitAttacked = aBruitsMage[0];

                // Bruit attaque spéciale du magicien
                oClone.GetComponent<DeplacementPerso>().bruitAttackSpecial = aBruitsMage[1];
                oClone.GetComponent<DeplacementPerso>().bruitAttackSpecial2 = aBruitsMage[2];

                // Bruit de mort du magicien
                oClone.GetComponent<DeplacementPerso>().bruitDead = aBruitsMage[3];
            }

            oClone.GetComponent<DeplacementPerso>().bruitSlash = aBruits[0];
            oClone.GetComponent<DeplacementPerso>().bruitForge = aBruits[1];

            oClone.GetComponent<DeplacementPerso>().txtConstruireArme = txtConstruireArme;
            oClone.GetComponent<DeplacementPerso>().txtRecolter = txtRecolter;
            oClone.GetComponent<DeplacementPerso>().imgConstruire = imgConstruire;
            oClone.GetComponent<DeplacementPerso>().oImgConstruire = oImgConstruire;
            oClone.GetComponent<DeplacementPerso>().aBarreVie = aBarreVie;
            oClone.GetComponent<DeplacementPerso>().vieVide = vieVide;
            oClone.GetComponent<DeplacementPerso>().aCrochetInv = aCrochetInv;
            oClone.GetComponent<DeplacementPerso>().aCaseRougeInv = aCaseRougeInv;
            oClone.GetComponent<DeplacementPerso>().oInventaire = oInventaire;
            oClone.GetComponent<DeplacementPerso>().txtSpectateur = txtSpectateur;

            // Fermer le panneau de début et ouvrir le panneau du jeu
            panDebut.SetActive(false);
            oHUD.SetActive(true);
        }


        /**
         * Vérifier si tous les joueurs ont chargé la scene de jeu
         * @param void
         * @return void
         * @author Exit Games
         */
        private bool CheckAllPlayerLoadedLevel() {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }
    
        /**
         * Vérifier si la partie est terminée
         * @param void
         * @return void
         * @author Exit Games et Pier-Olivier Bourdeau
         */
        private void CheckEndOfGame() {

            int iCompteur = 0;
            
            // Stocker le nombre de vie restante de tous les joueurs dans un tableau
            foreach (Player p in PhotonNetwork.PlayerList) {
                object lives;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives)) {
                    aTabVieJoueur[iCompteur] = (int)lives;
                }
                iCompteur++;
            }

            // Déterminer combien de joueur il reste en fonction du nombre de vie
            int iNbJoueurRestant = 4;
            for (int i=0; i<aTabVieJoueur.Length; i++) {
                if(aTabVieJoueur[i] <= 0) {
                    iNbJoueurRestant--;
                }
            }

            // S'il reste qu'un joueur en vie,
            if(iNbJoueurRestant <= 1 || bFin) {
                if (PhotonNetwork.IsMasterClient) {
                    StopAllCoroutines();
                }

                // Déterminer le joueur vainqueur
                string winner = "";
                int score = -1;

                foreach (Player p in PhotonNetwork.PlayerList) {
                    object lives;
                    if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives)) {
                        if ((int)lives > score) {
                            winner = p.NickName;
                            score = (int)lives;
                        }
                    }
                }

                // Afficher l'écran de fin de jeu
                StartCoroutine(EndOfGame(winner, score));
            }
        }

        /**
         * À la fin du countdown d'attente des joueurs, partir la partie
         * @param void
         * @return void
         * @author Exit Games
         */
        private void OnCountdownTimerIsExpired(){
            StartGame();
        }


        /**
         * Générer les ressources sur la carte aléatoirement
         * @param void
         * @return void
         * @author Vincent Gagnon
         */
        void GenererRessources() {

            foreach (Player p in PhotonNetwork.PlayerList) {
                nbJoueurConnecte++;
            }

            // Générer aléatoirement l'emplacement du bois
            Shuffle(aSpawnerBois);

            for (int iBois = 0; iBois < 4; iBois++) {
                GameObject oCloneBois = PhotonNetwork.InstantiateSceneObject("bois", aSpawnerBois[iBois].transform.position, Quaternion.Euler(aSpawnerBois[iBois].transform.eulerAngles));
                oCloneBois.transform.parent = aSpawnerBois[iBois].transform;
            }

            // Générer aléatoirement l'emplacement du fer
            Shuffle(aSpawnerFer);

            for (int iFer = 0; iFer < 5; iFer++) {
                GameObject oCloneFer = PhotonNetwork.InstantiateSceneObject("fer", aSpawnerFer[iFer].transform.position, Quaternion.Euler(aSpawnerFer[iFer].transform.eulerAngles));
                oCloneFer.transform.parent = aSpawnerFer[iFer].transform;
            }

            // Générer aléatoirement l'emplacement du cuir
            Shuffle(aSpawnerCuir);

            for (int iCuir = 0; iCuir < 4; iCuir++) {
                GameObject oCloneCuir = PhotonNetwork.InstantiateSceneObject("cuir", aSpawnerCuir[iCuir].transform.position, Quaternion.Euler(aSpawnerCuir[iCuir].transform.eulerAngles));
                oCloneCuir.transform.parent = aSpawnerCuir[iCuir].transform;
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
            if (iSecondeRestante < 10) {
                txtTimer.text = iMinuteRestante + ":0" + iSecondeRestante;
            }
            else {
                txtTimer.text = iMinuteRestante + ":" + iSecondeRestante;
            }

            // Animer le temps restant
            imgTimer.fillAmount = (float)(iMinuteRestante * 60 + iSecondeRestante) / iSecondeTotal;

            // S'il ne reste plus de temps, annuler l'appel de la fonction
            if (iMinuteRestante == 0 && iSecondeRestante == 0) {
                CancelInvoke();

                // Terminer la partie
                bFin = true;
                CheckEndOfGame();
            }
        }
    }
}
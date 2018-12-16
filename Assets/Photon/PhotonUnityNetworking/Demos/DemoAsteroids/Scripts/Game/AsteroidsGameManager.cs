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
    public class AsteroidsGameManager : MonoBehaviourPunCallbacks
    {
        public static AsteroidsGameManager Instance = null;

        public Text InfoText;
        public GameObject panDebut;
        public GameObject[] aSpawnPoint;
        public GameObject oHUD;
        public GameObject panFin;
        public Text txtGagnant;

        // Variables du perso
        public AudioClip[] aBruits;
        private AudioSource audioSrc;

        public GameObject txtConstruireArme;
        public GameObject txtRecolter;
        public Image imgConstruire;
        public GameObject oImgConstruire;
        public GameObject[] aBarreVie;
        public Sprite vieVide;
        public GameObject[] aCrochetInv;
        public GameObject[] aCaseRougeInv;
        public GameObject oInventaire;
        public GameObject txtSpectateur;

        public GameObject[] aSpawnerBois; // Tous les points de spawn du bois
        public GameObject[] aSpawnerFer; // Tous les points de spawn du fer
        public GameObject[] aSpawnerCuir; // Tous les points de spawn du cuir

        #region UNITY

        public void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            InfoText.text = "En attentes des autres joueurs...";

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES

        private IEnumerator EndOfGame(string winner, int score)
        {
            oHUD.SetActive(false);
            panFin.SetActive(true);
            txtGagnant.text = "Le gagnant est " + winner;

            float timer = 5.0f;

            while (timer > 0.0f)
            {
                txtGagnant.text = "Le gagnant est " + winner;

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneRejoindre");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
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

        #endregion

        private void StartGame()
        {

            // Créer les ressources
            GenererRessources();


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
            oClone.GetComponent<DeplacementPerso>().bruitSlash = aBruits[0];
            oClone.GetComponent<DeplacementPerso>().bruitAttacked = aBruits[1];
            oClone.GetComponent<DeplacementPerso>().bruitDead = aBruits[2];
            oClone.GetComponent<DeplacementPerso>().bruitAttackSpecial = aBruits[3];
            oClone.GetComponent<DeplacementPerso>().bruitForge = aBruits[4];
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


            panDebut.SetActive(false);
            oHUD.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                //StartCoroutine(SpawnAsteroid()); 
            }
        }

        private bool CheckAllPlayerLoadedLevel()
        {
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
    
        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object lives;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
                {
                    if ((int) lives > 0)
                    {
                        allDestroyed = false;
                        break;
                    }
                }
            }

            if (allDestroyed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;

                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                    }
                }

                StartCoroutine(EndOfGame(winner, score));
            }
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }

        /**
         * Générer les ressources sur la carte aléatoirement
         * @param void
         * @return void
         * @author Vincent Gagnon
         */
        void GenererRessources() {
            // Générer aléatoirement l'emplacement du bois
            Shuffle(aSpawnerBois);

            for (int iBois = 0; iBois < aSpawnerBois.Length / 2; iBois++) {
                GameObject oCloneBois = PhotonNetwork.Instantiate("bois", aSpawnerBois[iBois].transform.position, Quaternion.Euler(aSpawnerBois[iBois].transform.eulerAngles));
                oCloneBois.transform.parent = aSpawnerBois[iBois].transform;
            }

            // Générer aléatoirement l'emplacement du fer
            Shuffle(aSpawnerFer);

            for (int iFer = 0; iFer < aSpawnerFer.Length / 2; iFer++) {
                GameObject oCloneFer = PhotonNetwork.Instantiate("fer", aSpawnerFer[iFer].transform.position, Quaternion.Euler(aSpawnerFer[iFer].transform.eulerAngles));
                oCloneFer.transform.parent = aSpawnerFer[iFer].transform;
            }

            // Générer aléatoirement l'emplacement du cuir
            Shuffle(aSpawnerCuir);

            for (int iCuir = 0; iCuir < aSpawnerCuir.Length / 2; iCuir++) {
                GameObject oCloneCuir = PhotonNetwork.Instantiate("cuir", aSpawnerCuir[iCuir].transform.position, Quaternion.Euler(aSpawnerCuir[iCuir].transform.eulerAngles));
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
    }
}
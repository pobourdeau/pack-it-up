// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;
        public Sprite[] PlayerFlag;
        public Toggle PlayerCharacterKnight;
        public Toggle PlayerCharacterMage;
        public GameObject lblPerso;

        public static string nomPerso = "knight";
        private int ownerId;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);

                // Ne pas afficher les checkbox de l'autre joueur
                PlayerCharacterKnight.gameObject.SetActive(false);
                PlayerCharacterMage.gameObject.SetActive(false);
                lblPerso.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}, {AsteroidsGame.PLAYER_LIVES, AsteroidsGame.PLAYER_MAX_LIVES}, {"idPlayer", ownerId } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                // Choisir le personnage
                PlayerCharacterKnight.onValueChanged.AddListener(delegate {
                    if (PlayerCharacterKnight.isOn) {
                        PlayerCharacterMage.isOn = false;
                        nomPerso = "knight";
                    }
                    else {
                        PlayerCharacterMage.isOn = true;
                        nomPerso = "mage";
                    }
                });

                PlayerCharacterMage.onValueChanged.AddListener(delegate {
                    if (PlayerCharacterMage.isOn) {
                        PlayerCharacterKnight.isOn = false;
                        nomPerso = "mage";
                    }
                    else {
                        PlayerCharacterKnight.isOn = true;
                        nomPerso = "knight";
                    }
                });

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }


        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    //print(p.GetPlayerNumber());
                    PlayerColorImage.sprite = PlayerFlag[AsteroidsGame.GetColor(p.GetPlayerNumber())];
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Prêt!" : "Prêt?";
            PlayerReadyImage.enabled = playerReady;
        }
    }
}
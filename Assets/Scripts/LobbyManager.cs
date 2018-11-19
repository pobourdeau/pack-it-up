using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager {

    public GameObject Lobby;

    public override void OnStartHost() {
        Lobby.SetActive(true);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoindreLobby : MonoBehaviour {

    LobbyManager lobbyManager;
    public GameObject PrefabJoindre;
    public GameObject parentListe;

	// Use this for initialization
	void Start () {
        lobbyManager = GameObject.FindGameObjectWithTag("networkManager").GetComponent<LobbyManager>();
	}
	
	public void RafraichirListe() {
        if(lobbyManager == null) {
            lobbyManager = GameObject.FindGameObjectWithTag("networkManager").GetComponent<LobbyManager>();
        }

        if(lobbyManager.matchMaker == null) {
            lobbyManager.StartMatchMaker();
        }

        lobbyManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, onMatchList);

    }

    private void onMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {

        if (!success) {
            print("Refresh liste");
        }
        
        foreach(MatchInfoSnapshot mactch in matchList) {
            GameObject oClone = Instantiate(PrefabJoindre);
            oClone.transform.SetParent(parentListe.transform);
        }
    }
}

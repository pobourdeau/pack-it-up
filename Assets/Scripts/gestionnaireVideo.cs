using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class gestionnaireVideo : MonoBehaviour {

    public GameObject cam;
    public VideoPlayer video;
    public GameObject oVideo;
    public GameObject oCamPrinc;
    public GameObject menu;

	// Use this for initialization
	void Start () {
        Invoke("LancerVideo", 0);
        InvokeRepeating("VerifierVideo", 2f, 1f);
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void LancerVideo() {
        video.Play();
    }

    public void VerifierVideo() {
        if(video.isPlaying == false) {
            video.Pause();
            CancelInvoke("VerifierVideo");

            cam.SetActive(false);
            oCamPrinc.SetActive(true);
            oVideo.SetActive(false);
            menu.SetActive(true);
        }
    }
}

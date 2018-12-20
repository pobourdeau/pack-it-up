using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Déplacement de la caméra pour qu'elle suive le personnage en tout temps
 * @author Pier-Olivier Bourdeau
 * @version 2018-11-12
 */
public class DeplacementCam : MonoBehaviour {

    //Valeur float pour la caméra
    [SerializeField]
    public float distance = 7.0f;

    //Valeur float pour la caméra
    [SerializeField]
    public float height = 3.0f;

    //Valeur float pour la caméra
    [SerializeField]
    private float heightSmoothLag = 0.3f;

    //Valeur Vector 3 pour la caméra
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;

    //Valeur de suivi de caméra
    [SerializeField]
    private bool followOnStart = false;

    Transform cameraTransform;//Camera
    public bool isFollowing;//Suivi ou non
    private float heightVelocity;//Hauteur vélocité pour la caméra
    private float targetHeight = 100000.0f;//Hauteur de la cible

    public Vector3 posInitiale;//La position de base de la caméra


    void Start() {

        // Start following the target if wanted.
        if (followOnStart) {
            OnStartFollowing();
        }

    }

    /**
  * Détection si la caméra existe ou pas 
  * @param void
  * @return void
  * 
  * Pier-Olivier Bourdeau
  */
    void LateUpdate() {

        //Si la caméra est inexistante
        if (cameraTransform == null && isFollowing) {
            OnStartFollowing();
        }

        //Si la caméra suit déjà
        if (isFollowing) {
            Apply();
        }
    }

    /**
     * On fait suivre la caméra
     * @param void
     * @return void
     * 
     * Pier-Olivier Bourdeau
     */
    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        //La caméra suit maintenant
        isFollowing = true;

        Cut();
    }

    /**
  * La caméra s'ajuste à chacun des personnage avec les bonnes options
  * @param void
  * @return void
  * 
  * Pier-Olivier Bourdeau
  */
    void Apply() {
        Vector3 targetCenter = transform.position + centerOffset;

        // Calculate the current & target rotation angles
        float originalTargetAngle = transform.eulerAngles.y;
        float currentAngle = cameraTransform.eulerAngles.y;

        // Adjust real target angle when camera is locked
        float targetAngle = originalTargetAngle;
        currentAngle = targetAngle;
        targetHeight = targetCenter.y + height;


        // Damp the height
        float currentHeight = cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);

        // Convert the angle into a rotation, by which we then reposition the camera
        //Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        cameraTransform.position = targetCenter;
        cameraTransform.position += Vector3.back * distance;

        // Set the height of the camera
        cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);

        // Always look at the target
        SetUpRotation(targetCenter);
    }

    /**
  * Ajustement de la hauteur si nécéssaire
  * @param void
  * @return void
  * 
  * Pier-Olivier Bourdeau
  */
    void Cut() {
        float oldHeightSmooth = heightSmoothLag;

        heightSmoothLag = 0.001f;

        Apply();

        heightSmoothLag = oldHeightSmooth;
    }

    /**
  * Rotation de la caméra addéquate
  * @param void
  * @return void
  * 
  * Pier-Olivier Bourdeau
  */
    void SetUpRotation(Vector3 centerPos) {

        Vector3 cameraPos = cameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;

        // Generate base rotation only around y-axis
        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
    }

    public void PositionnerCameraMort() {
        cameraTransform.position = new Vector3(-205f, 25f, -30f);
        cameraTransform.eulerAngles = new Vector3(14f, 0f, 0f);
    }
}

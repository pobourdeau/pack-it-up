using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Déplacement de la caméra pour qu'elle suive le personnage en tout temps
 * @author Pier-Olivier Bourdeau
 * @version 2018-11-12
 */
public class DeplacementCam : MonoBehaviour {

    [SerializeField]
    public float distance = 7.0f;

    [SerializeField]
    public float height = 3.0f;

    [SerializeField]
    private float heightSmoothLag = 0.3f;

    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;

    [SerializeField]
    private bool followOnStart = false;

    Transform cameraTransform;
    public bool isFollowing;
    private float heightVelocity;
    private float targetHeight = 100000.0f;

    public Vector3 posInitiale;


    void Start() {

        // Start following the target if wanted.
        if (followOnStart) {
            OnStartFollowing();
        }

    }

    void LateUpdate() {

        if (cameraTransform == null && isFollowing) {
            OnStartFollowing();
        }

        if (isFollowing) {
            Apply();
        }
    }

    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        isFollowing = true;

        Cut();
    }

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


    void Cut() {
        float oldHeightSmooth = heightSmoothLag;

        heightSmoothLag = 0.001f;

        Apply();

        heightSmoothLag = oldHeightSmooth;
    }


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

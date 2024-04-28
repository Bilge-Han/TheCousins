using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSens = 2f;
    float cameraVerticalRotation = 0f;

    private void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSens;
        float inputY = Input.GetAxis("Mouse Y") * mouseSens;

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        player.Rotate(Vector3.up, inputX);
    }
}

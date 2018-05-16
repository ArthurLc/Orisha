/*
* @ArthurLacour
* @CameraFPS.cs
* @16/05/2018
* @ - Le Script s'attache sur un objet quelconque.
*   
* Détails :
*   - Le script permet le contrôle de la camera en FPS pour faire le trailer.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFPS : MonoBehaviour {
    [Header("Links")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera fpsCamera;
    [Header("Parameters")]
    [SerializeField] float mouseSpeed = 3.0f;
    [SerializeField] float moveSpeed = 6.0f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            fpsCamera.transform.position = playerCamera.transform.position;
            fpsCamera.transform.rotation = playerCamera.transform.rotation;

            fpsCamera.enabled = !fpsCamera.enabled;
        }

        // Read the mouse input axis
        transform.rotation *= Quaternion.Euler(new Vector3(Input.GetAxis("Keyboard_CamY") * mouseSpeed * Time.fixedDeltaTime, -Input.GetAxis("Keyboard_CamX") * mouseSpeed * Time.fixedDeltaTime, 0));
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));

        if (Input.GetKey(KeyCode.Keypad8)) //Up
        {
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.Keypad2)) //Down
        {
            transform.position -= transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.Keypad4)) //Left
        {
            transform.position -= transform.right * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.Keypad6)) //Right
        {
            transform.position += transform.right * moveSpeed * Time.fixedDeltaTime;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}

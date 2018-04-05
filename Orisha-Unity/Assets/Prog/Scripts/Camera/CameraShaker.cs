using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
* @JulienLopez
* @CameraShaker.cs
* @04/04/2018
* @Le script s'attache sur l'objet qui possède le bain du player
*   - Il permet d'appeller de n'importe où une coroutine lancement le shake de camera actuelle avec la durée et l'amplitude voulu
* 
*/

public class CameraShaker : MonoBehaviour {

    [SerializeField, Tooltip("It's a button ! (launch the shaking in play mode or if it's true when enter in play mode he play on awake")]
        private bool launchDebug = false;

    private CinemachineBrain brain;

    private CameraShaker instance;

    public CameraShaker Instance
    {
        get
        {
            if (instance == null)
                instance = this;
                
            return instance;
        }
    }

    private void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }

    private void Update()
    {
        if(launchDebug)
        {
            ShakeActualCam( 0.5f, 1.0f, 0.01f);
            Debug.Log("Shake it!");
            launchDebug = false;
        }
    }

    /// <summary>
    /// Function to call shaking on the actual virtual cam
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="magnitudeX"></param>
    /// <param name="magnitudeY"></param>
    public void ShakeActualCam(float duration, float magnitudeX, float magnitudeY)
    {
        StartCoroutine(Shake(brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>(), duration, magnitudeX, magnitudeY));
    }

    /// <summary>
    /// Coroutine of shaking
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="duration"></param>
    /// <param name="magnitudeX"></param>
    /// <param name="magnitudeY"></param>
    /// <returns></returns>
    private IEnumerator Shake(CinemachineFreeLook camera,float duration, float magnitudeX, float magnitudeY)
    {
        float originalPosX = camera.m_XAxis.Value;
        float originalPosY = camera.m_YAxis.Value;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            camera.m_XAxis.Value += Random.Range(-1.0f, 1.0f) * magnitudeX;
            camera.m_YAxis.Value += Random.Range(-1.0f, 1.0f) * magnitudeY;
            
            elapsed += Time.deltaTime;

            yield return null;

        }

        camera.m_XAxis.Value = originalPosX;
        camera.m_YAxis.Value = originalPosY;

        yield break;
    }
}

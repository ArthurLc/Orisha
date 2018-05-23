using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTransporter : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private MeshRenderer platform;
    enum EmissiveMode
    {
        Pulse,
        Shine
    } EmissiveMode emissiveMode;

    [Header("Parameters")]
	[SerializeField] float GlobalDuration = 10.0f;
	private bool isMoving = false;
	private bool isAtStart = true;

	private vd_Player.CharacterInitialization Ci;
	private Rigidbody playerRB;

	public AnimationCurve curveMoveY;
	public CallTransporter StartPoint;
	public CallTransporter EndPoint;
    [SerializeField] private MeshCollider platformMesh;

    [SerializeField] private Color PulseColor;
    [SerializeField] private Color ShineColor;
    [SerializeField][Range(-10.0f, 10.0f)] private float PulseEmissiveMax = 1.0f;
    [SerializeField] private float PulseSpeed = 1.0f;
    [SerializeField] private float LerpSpeed = 1.0f;

    private float interpo = 0.0f;

	private bool needToLeave = false;
	private BoxCollider bc;

    private Material prefabMaterial;

    float emissiveValue;
    int direction;

    public bool IsMoving
    {
        get { return isMoving; }
    }
    public bool IsAtStart
    {
        get { return isAtStart; }
    }


    private void Start()
	{
		bc = GetComponent<BoxCollider>();
		for (int i = 0; i < curveMoveY.length; i++) {
			curveMoveY.SmoothTangents (i, 1.1f);
		}

        StartPoint.gameObject.SetActive(false);
        EndPoint.gameObject.SetActive(true);

        SetEmissiveMode(EmissiveMode.Pulse);
        prefabMaterial = platform.sharedMaterial;
        platform.sharedMaterial = new Material(prefabMaterial);
        platform.sharedMaterial.name = transform.name;
        emissiveValue = 0.0f;
        direction = 1;
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        platform.sharedMaterial = prefabMaterial;
    }

    private void Update()
    {
        if (!isAtStart && !isMoving && !needToLeave)
        {
            if (Vector3.Distance(Ci.PlayerTr.position, transform.position) <= 0.25f)
            {
                StartPoint.transform.parent = null;
                EndPoint.transform.parent = null;
                Ci.transform.parent = transform;
                isMoving = true;
                Ci.PlayerController.isPlayingCinematic = false;
                playerRB.isKinematic = true;
                Ci.Anim.SetFloat("Speed", 0.0f);
                Ci.PlayerTr.position = transform.position;
                Ci.PlayerTr.localRotation = Quaternion.Euler(0, Ci.PlayerTr.localRotation.eulerAngles.y, 0);
                isAtStart = true;
            }
            else
            {
                playerRB.isKinematic = (Ci.PlayerTr.position.y <= transform.position.y);
                Ci.PlayerController.isPlayingCinematic = true;
                Ci.PlayerTr.LookAt(transform);
                Ci.Anim.SetFloat("Speed", 0.5f);
            }
        }

        if (playerRB && needToLeave && Ci)
        {
            Vector3 pos2D, playerPos2D;
            pos2D = transform.position;
            playerPos2D = Ci.PlayerTr.position;
            pos2D.y = playerPos2D.y = 0.0f;
            if (Vector3.Distance(playerPos2D, pos2D) >= bc.size.x + 0.5f)
            {
                needToLeave = false;
                Ci = null;
                //Debug.Log("tu peut reprendre la plateforme !");	
            }
        }

        MoveTransporter();
        UpdateEmissive();
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
        {
            if (Ci == null)
                Ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();
            if (playerRB == null)
                playerRB = Ci.Rb;

            if (isAtStart && !needToLeave && !isMoving)
            {
                SetEmissiveMode(EmissiveMode.Shine);

                isAtStart = false;
                Ci.FreezeInputs();
                playerRB.isKinematic = true;
                Ci.PlayerTr.LookAt(transform);
                Ci.PlayerController.isPlayingCinematic = true;
                Ci.Anim.SetFloat("Speed", 0.5f);

                StartPoint.gameObject.SetActive(false);
                EndPoint.gameObject.SetActive(false);

                platformMesh.enabled = false;
            }
        }
	}


	private void MoveTransporter()
	{
        if (isMoving)
        {
			interpo += Time.deltaTime;

            if (Ci != null)
            {
                Ci.PlayerTr.position = transform.position;
                Ci.PlayerTr.localRotation = Quaternion.Euler(0, Ci.PlayerTr.localRotation.eulerAngles.y, 0);
                Ci.Anim.SetFloat("Speed", 0.0f);
            }

			if (interpo < GlobalDuration)
            {
				transform.position = Vector3.Lerp(StartPoint.transform.position, EndPoint.transform.position, (interpo / GlobalDuration));
				transform.position = new Vector3 (transform.position.x, transform.position.y * curveMoveY.Evaluate (interpo / GlobalDuration)
												 , transform.position.z);
				//Debug.Log(transform.position + " | " + interpo);

			}
            else
            {

				transform.position = EndPoint.transform.position;
                
				//Debug.Log("End : " + transform.position + " | " + interpo);

				//Arrivé
				interpo = 0.0f;
				isMoving = false;
				CallTransporter tmpCallTransporter = StartPoint;
				StartPoint = EndPoint;
				EndPoint = tmpCallTransporter;

                StartPoint.transform.parent = transform;
				EndPoint.transform.parent = transform;

				Keyframe[] keys = curveMoveY.keys;
				int keyCount = keys.Length;
				WrapMode postWrapmode = curveMoveY.postWrapMode;
				curveMoveY.postWrapMode = curveMoveY.preWrapMode;
				curveMoveY.preWrapMode = postWrapmode;
				for(int i = 0; i < keyCount; i++ )
				{
					Keyframe K = keys[i];
					K.time = 1.0f - K.time;
					float tmp = -K.inTangent;
					K.inTangent = -K.outTangent;
					K.outTangent = tmp;
					keys[i] = K;
				}

				curveMoveY.keys = keys;

                //Debug.Log ("Tu ne peut pas prendre la platerforme");

                if (Ci != null)
                {
                    Ci.transform.parent = null;
                    Ci.UnfreezeInputs();
                    playerRB.isKinematic = false;
                }
                isAtStart = true;

                needToLeave = (Ci != null);
                SetEmissiveMode(EmissiveMode.Pulse);

                platformMesh.enabled = true;

                StartPoint.gameObject.SetActive(false);
                EndPoint.gameObject.SetActive(true);
            }

            
        }
        else if (Ci != null && playerRB != null)
        {
            if (Vector3.Distance(Ci.PlayerTr.position, transform.position) <= 0.1f)
            {
                Ci.transform.parent = null;
                Ci.UnfreezeInputs();
                playerRB.isKinematic = false;
                Ci.PlayerController.isPlayingCinematic = false;
            }
        }
	}

    private void SetEmissiveMode(EmissiveMode _newEmissiveMode)
    {
        emissiveMode = _newEmissiveMode;
        StopAllCoroutines();

        switch (emissiveMode)
        {
            case EmissiveMode.Pulse:
                platform.sharedMaterial.SetColor("_EmissionColor", PulseColor);
                break;
            case EmissiveMode.Shine:
                platform.sharedMaterial.SetColor("_EmissionColor", ShineColor);
                break;
            default:
                break;
        }
    }
    private void UpdateEmissive()
    {
        Color finalColor;

        switch (emissiveMode)
        {
            case EmissiveMode.Pulse:
                emissiveValue += Time.fixedDeltaTime * PulseSpeed * direction;
                if (emissiveValue > PulseEmissiveMax || emissiveValue < 0.0f) {
                    direction = -direction;
                }

                finalColor = PulseColor * Mathf.LinearToGammaSpace(emissiveValue);
                platform.sharedMaterial.SetColor("_EmissionColor", finalColor);
                break;
            case EmissiveMode.Shine:
                if (emissiveValue < PulseEmissiveMax)
                {
                    emissiveValue += Time.fixedDeltaTime * PulseSpeed;

                    finalColor = ShineColor * Mathf.LinearToGammaSpace(emissiveValue);
                    platform.sharedMaterial.SetColor("_EmissionColor", finalColor);
                }
                else if(emissiveValue > PulseEmissiveMax)
                {
                    finalColor = ShineColor * Mathf.LinearToGammaSpace(PulseEmissiveMax);
                    platform.sharedMaterial.SetColor("_EmissionColor", finalColor);
                }

                break;
            default:
                break;
        }
    }

    public void CallTransporter(Collider _col)
    {
        StartPoint.gameObject.SetActive(false);
        EndPoint.gameObject.SetActive(false);

        StartPoint.transform.parent = null;
        EndPoint.transform.parent = null;
        isMoving = true;
        isAtStart = true;

        platformMesh.enabled = false;
    }
}

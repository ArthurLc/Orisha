/*
* @ArthurLacour
* @SandBridge.cs
* @27/03/2018
* @ - Le Script s'attache à un GameObject vide parent.
*   
* Contient :
*   - Editeur construction du SandBridge.
* 
* A faire:
*/

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SandBridge))]
public class SandBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SandBridge SB = target as SandBridge;
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = (SB.MeshsList.childCount == 0);
        if (GUILayout.Button("Build SandBridge")) {
            BuildBridge(SB);
        }
        GUI.enabled = (SB.MeshsList.childCount != 0);
        if (GUILayout.Button("Remove SandBridge")) {
            RemoveBridge(SB.MeshsList);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void BuildBridge(SandBridge _sb)
    {
        //Placement des poteaux
        _sb.FirstBridgePole.transform.LookAt(_sb.SecondBridgePole);
        _sb.FirstBridgePole.transform.localRotation = Quaternion.Euler(new Vector3(0, _sb.FirstBridgePole.transform.localEulerAngles.y, 0));
        _sb.SecondBridgePole.transform.LookAt(_sb.FirstBridgePole);
        _sb.SecondBridgePole.transform.localRotation = Quaternion.Euler(new Vector3(0, _sb.SecondBridgePole.transform.localEulerAngles.y, 0));

        //Gestion des rotations du pont
        _sb.MeshsList.transform.LookAt(_sb.ArrivalPonton);
        _sb.MeshsList.transform.localEulerAngles = new Vector3(_sb.MeshsList.transform.localEulerAngles.x, _sb.MeshsList.transform.localEulerAngles.y, 0.0f);
        _sb.GroundCollider.transform.rotation = _sb.MeshsList.transform.rotation;
        _sb.LeftWallCollider.transform.rotation = _sb.GroundCollider.transform.rotation;
        _sb.RightWallCollider.transform.rotation = _sb.GroundCollider.transform.rotation;

        //Placement des colliders
        _sb.GroundCollider.transform.position = ((_sb.FirstBridgePole.position - _sb.SecondBridgePole.position) / 2) + _sb.SecondBridgePole.position;
        _sb.LeftWallCollider.transform.position = (((_sb.FirstBridgePole.position - _sb.SecondBridgePole.position) / 2) + _sb.SecondBridgePole.position) - (_sb.GroundCollider.transform.right * 3.0f);
        _sb.RightWallCollider.transform.position = (((_sb.FirstBridgePole.position - _sb.SecondBridgePole.position) / 2) + _sb.SecondBridgePole.position) + (_sb.GroundCollider.transform.right * 3.0f);
        //Scale des colliders
        float bridgeLenght = Vector3.Distance(_sb.StartPonton.position, _sb.ArrivalPonton.position);
        _sb.GroundCollider.transform.localScale = new Vector3(_sb.GroundCollider.transform.localScale.x, _sb.GroundCollider.transform.localScale.y, bridgeLenght);
        _sb.LeftWallCollider.transform.localScale = new Vector3(_sb.LeftWallCollider.transform.localScale.x, _sb.LeftWallCollider.transform.localScale.y, bridgeLenght);
        _sb.RightWallCollider.transform.localScale = new Vector3(_sb.RightWallCollider.transform.localScale.x, _sb.RightWallCollider.transform.localScale.y, bridgeLenght);


        //Placement des marches
        float idx = 1.1f;
        Transform currentStepRef = _sb.StartPonton;
        for (int i = 0; Vector3.Distance(currentStepRef.position, _sb.ArrivalPonton.position) > idx; i++) {
            GameObject newStep = Instantiate(_sb.StepElement, _sb.MeshsList);
            newStep.transform.localRotation = Quaternion.identity;
            newStep.transform.localPosition = (Vector3.forward * 0.5f) + ((Vector3.forward * idx) * i);
            currentStepRef = newStep.transform;
        }

    }
    private void RemoveBridge(Transform _meshList)
    {
        for(int i = _meshList.childCount; i > 0; i--) {
            DestroyImmediate(_meshList.GetChild(i - 1).gameObject);
        }
    }
}

#endif

public class SandBridge : MonoBehaviour {
    
    [Header("Prefabs")]
    [SerializeField] private GameObject stepElement; //Le prefab de la Marche
    [Header("Links")]
    [SerializeField] private Transform firstBridgePole; //Transform parent des poteaux
    [SerializeField] private Transform secondBridgePole; //Transform parent des poteaux
    [SerializeField] private Transform startPonton; //Départ du pont
    [SerializeField] private Transform arrivalPonton; //Arrivé du pont
    [SerializeField] private Transform stepList; //Le parent des GameObjects marches du pont. (Qui apparaîtront..)
    [SerializeField] private Collider groundCollider; //GameObject qui porte le collider du sol du pont.
    [SerializeField] private Collider leftWallCollider; //GameObject qui porte le collider du mur gauche.
    [SerializeField] private Collider rightWallCollider; //GameObject qui porte le collider du mur droit.

    #region Getteurs
    public GameObject StepElement
    {
        get { return stepElement; }
    }
    public Transform MeshsList
    {
        get { return stepList; }
    }

    public Transform FirstBridgePole
    {
        get { return firstBridgePole; }
    }
    public Transform SecondBridgePole
    {
        get { return secondBridgePole; }
    }
    public Transform StartPonton
    {
        get { return startPonton; }
    }
    public Transform ArrivalPonton
    {
        get { return arrivalPonton; }
    }

    public Collider GroundCollider
    {
        get { return groundCollider; }
    }
    public Collider LeftWallCollider
    {
        get { return leftWallCollider; }
    }
    public Collider RightWallCollider
    {
        get { return rightWallCollider; }
    }
    #endregion
}


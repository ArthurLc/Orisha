/*
* @AmbreLacour
* @BridgeGenerator.cs
* @14/02/2018
* @Script éditeur qui permet de générer des ponts
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BridgeGeneratorWindow : EditorWindow
{
    private Transform tr_Bridge;
    private Rigidbody rb_BridgeBegin;
    private Rigidbody rb_BridgeEnd;
    private GameObject prefab_planks;

    private float spaceBetweenPlanks = 2.2f;
    private float spaceBetweenFirstPillarsAndFirstPlanks = 0.5f;

    [MenuItem("Tools/Bridge Generator")]
    static void OpenWindow()
    {
        BridgeGeneratorWindow currentWindow = GetWindow<BridgeGeneratorWindow>(true, "Bridge Generator");
        currentWindow.Show();
        currentWindow.Repaint();
    }

    private void OnGUI() // Display tool
    {
        SelectInformationForGeneration();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private void SelectInformationForGeneration()
    {
        EditorGUILayout.LabelField("Generation of the bridge", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        tr_Bridge = EditorGUILayout.ObjectField("Bridge folder in scene", tr_Bridge, typeof(Transform), true) as Transform;
        rb_BridgeBegin = EditorGUILayout.ObjectField("Bridge entry in scene", rb_BridgeBegin, typeof(Rigidbody), true) as Rigidbody;
        rb_BridgeEnd = EditorGUILayout.ObjectField("Bridge exit in scene", rb_BridgeEnd, typeof(Rigidbody), true) as Rigidbody;

        EditorGUILayout.Space();

        prefab_planks = EditorGUILayout.ObjectField("Plank prefab", prefab_planks, typeof(GameObject), false) as GameObject;

        EditorGUILayout.Space();

        spaceBetweenPlanks = EditorGUILayout.FloatField("Space between planks", spaceBetweenPlanks);
        spaceBetweenFirstPillarsAndFirstPlanks = EditorGUILayout.FloatField("Space between entry bridge and planks", spaceBetweenFirstPillarsAndFirstPlanks);
    }

    private void Generate()
    {
        if (rb_BridgeBegin == null || rb_BridgeEnd == null || tr_Bridge == null || prefab_planks == null)
            return;

        Vector3 dirBridge = rb_BridgeEnd.transform.position - rb_BridgeBegin.transform.position;
        float bridgeSize = dirBridge.magnitude;
        dirBridge.Normalize();

        int nbrBoard = (int)System.Math.Round((bridgeSize / spaceBetweenPlanks));


        //Generate board
        List<GameObject> Boards = new List<GameObject>();
        for (int i = 0; i < nbrBoard; i++)
        {

            GameObject Board = Instantiate(prefab_planks, rb_BridgeBegin.transform.position + ((i + spaceBetweenFirstPillarsAndFirstPlanks) * dirBridge * spaceBetweenPlanks), Quaternion.identity, tr_Bridge);
            Board.SetActive(true);
            Board.transform.LookAt(Board.transform.position + dirBridge, Vector3.up);
            Board.transform.Rotate(Vector3.up, 90.0f);


            HingeJoint leftJoin = Board.AddComponent<HingeJoint>();
            leftJoin.anchor = new Vector3(0.0f, 0.0f, 0.0f);
            leftJoin.axis = Vector3.up;

            leftJoin.connectedMassScale = 1;
            leftJoin.massScale = 1;

            if (i == 0)            //First board
                leftJoin.connectedBody = rb_BridgeBegin;
            else                   // Others
                leftJoin.connectedBody = Boards[i - 1].GetComponent<Rigidbody>();

            Boards.Add(Board);
        }

        // Link between last plank and exit pillars
        rb_BridgeEnd.mass = 100;
        rb_BridgeEnd.useGravity = false;
        rb_BridgeEnd.isKinematic = true;
        FixedJoint endJoin = rb_BridgeEnd.gameObject.AddComponent<FixedJoint>();
        endJoin.connectedBody = Boards[Boards.Count - 1].GetComponent<Rigidbody>();
        endJoin.connectedMassScale = 1;
        endJoin.massScale = 1;
    }

}

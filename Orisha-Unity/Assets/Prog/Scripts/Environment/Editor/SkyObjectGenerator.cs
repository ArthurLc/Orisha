/*
@: Made by: Romain Seurot
@: Description: 
*   Tool script to generate sky objects
*
@: Date of Creation: 17/01/2018

@: Date of Last modification: 17/01/2018
@: Last modificator: Romain Seurot
*/


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class SkyObjectGenerator : EditorWindow
{
    private int numberOfSkyObjects = 1;
    private bool isFoldoutOpen = false;
    private GameObject[] prefabsSkyObjects;
    private GameObject skyObjectsParent;

    private float XOrigineAxis = 0.0f;
    private float ZOrigineAxis = 0.0f;

    private int numberOfObjects = 20;

    private float minHeigthToGenerate = 10.0f;
    private float maxHeigthToGenerate = 100.0f;

    private float minRadiusToGenerate = 40.0f;
    private float maxRadiusToGenerate = 100.0f;

    private float minRotationSpeedToGenerate = -1.0f;
    private float maxRotationSpeedToGenerate = 1.0f;

    private string namePrefabToRemplace = "";
    private GameObject prefabToRemplace;

    [MenuItem("Tools/SkyObjects Generator")]
    static void OpenWindow()
    {
        SkyObjectGenerator currentWindow = EditorWindow.GetWindow<SkyObjectGenerator>(true, "SkyObjects Generator");
        currentWindow.Show();
        currentWindow.Repaint();
    }

    private void OnGUI() // Display tool
    {
        SelectFolderInScene();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        SelectInformationForGeneration();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
        if (GUILayout.Button("Remplace"))
        {
            Remplace();
        }
    }
    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private void SelectFolderInScene()
    {
        skyObjectsParent = GameObject.Find("SkyObjects");
    }

    private void SelectInformationForGeneration()
    {
        EditorGUILayout.LabelField("Generation of sky objects", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        numberOfSkyObjects = EditorGUILayout.IntField("Number of type of object in the sky", numberOfSkyObjects);

        isFoldoutOpen = EditorGUILayout.Foldout(isFoldoutOpen, "Type of objects");
        if (isFoldoutOpen)
        {
            VerifArray();

            for (int i = 0; i < numberOfSkyObjects; i++)
            {
                prefabsSkyObjects[i] = EditorGUILayout.ObjectField("Prefab object" + (i + 1), prefabsSkyObjects[i], typeof(GameObject), false) as GameObject;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        minHeigthToGenerate = EditorGUILayout.FloatField("Min Height", minHeigthToGenerate);
        maxHeigthToGenerate = EditorGUILayout.FloatField("Max Height", maxHeigthToGenerate);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        minRadiusToGenerate = EditorGUILayout.FloatField("Min Radius", minRadiusToGenerate);
        maxRadiusToGenerate = EditorGUILayout.FloatField("Max Radius", maxRadiusToGenerate);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        minRotationSpeedToGenerate = EditorGUILayout.Slider("Min Rotation Speed", minRotationSpeedToGenerate, -10.0f, 10.0f);
        maxRotationSpeedToGenerate = EditorGUILayout.Slider("Max Rotation Speed", minRotationSpeedToGenerate, -10.0f, 10.0f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        XOrigineAxis = EditorGUILayout.FloatField("X Origin rotation", XOrigineAxis);
        ZOrigineAxis = EditorGUILayout.FloatField("Z Origin rotation", ZOrigineAxis);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        numberOfObjects = EditorGUILayout.IntField("Number of object", numberOfObjects);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        namePrefabToRemplace = EditorGUILayout.TextField("Name GameObjects to remplace", namePrefabToRemplace);
        prefabToRemplace = EditorGUILayout.ObjectField("Prefab to remplace", prefabToRemplace, typeof(GameObject), false) as GameObject;
    }
    private void VerifArray()
    {
        if(prefabsSkyObjects == null)
        {
            prefabsSkyObjects = new GameObject[1];
        }
        if(numberOfSkyObjects != prefabsSkyObjects.Length)
        {
            GameObject[] tempNewArray = new GameObject[numberOfSkyObjects];
            prefabsSkyObjects = new GameObject[numberOfSkyObjects];

            for (int i = 0; i < numberOfObjects; i++)
            {
                if(prefabsSkyObjects[i] != null)
                    tempNewArray[i] = prefabsSkyObjects[i];
            }

            prefabsSkyObjects = new GameObject[numberOfSkyObjects];

            for (int i = 0; i < numberOfObjects; i++)
            {
                prefabsSkyObjects[i] = tempNewArray[i];
            }
        }
    }
    private void Generate()
    {
        if(skyObjectsParent == null)
        {
            skyObjectsParent = new GameObject("SkyObjects");
        }

        for( int i = 0; i < numberOfObjects; i++)
        {
            int m_random = UnityEngine.Random.Range(0, numberOfSkyObjects);

            GameObject tempNewObject = Instantiate(prefabsSkyObjects[m_random]);
            tempNewObject.transform.parent = skyObjectsParent.transform;

            float m_randomHeight = UnityEngine.Random.Range(minHeigthToGenerate, maxHeigthToGenerate);
            float m_randomRadius = UnityEngine.Random.Range(minRadiusToGenerate, maxRadiusToGenerate);
            float m_randomSpeed = UnityEngine.Random.Range(minRotationSpeedToGenerate, maxRotationSpeedToGenerate);

            tempNewObject.transform.position = new Vector3(0.0f, m_randomHeight, 0.0f);

            Vector3 tempDir = new Vector3(UnityEngine.Random.Range(0.0f, 100.0f), UnityEngine.Random.Range(0.0f, 100.0f), UnityEngine.Random.Range(0.0f, 100.0f));
            tempDir = Vector3.Normalize(tempDir);

            tempNewObject.transform.position += (tempDir * m_randomRadius);

            if (tempNewObject.GetComponent<SkyObjectRotation>() == null)
            {
                tempNewObject.AddComponent<SkyObjectRotation>();
            }

            tempNewObject.GetComponent<SkyObjectRotation>().radius = m_randomRadius;
            tempNewObject.GetComponent<SkyObjectRotation>().speed = m_randomSpeed;
            tempNewObject.GetComponent<SkyObjectRotation>().Origin = new Vector3(XOrigineAxis, 0.0f, ZOrigineAxis);
        }
    }
    private void Remplace()
    {
        GameObject newIsland;
        string nameToAdd = namePrefabToRemplace;

        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == nameToAdd)
            {
                newIsland = Instantiate(prefabToRemplace, go.transform) as GameObject;
                newIsland.transform.parent = go.transform.parent;

                SkyObjectRotation oldScript = go.GetComponent<SkyObjectRotation>();
                SkyObjectRotation newScript = newIsland.AddComponent<SkyObjectRotation>();
                newScript.radius = oldScript.radius;
                newScript.speed = oldScript.speed;
                newScript.speedOnItSelf = oldScript.speedOnItSelf;
                newScript.Origin = oldScript.Origin;

                Destroy(go);
            }
        }
    }
}

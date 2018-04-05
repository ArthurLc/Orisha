using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class Orisha_VertexPainter : EditorWindow
{
    private static Orisha_VertexPainter currentWindow;

    private GameObject go;
    private Collider collider;
    private MeshFilter mf;
    private Mesh mesh;
    private Mesh currentMesh;
    private Renderer renderer;
    private Vector3[] vertices;
    private Color[] originalColors, newColors;
    private Material originalMaterial;

    private static Material newMaterial;

    private int numActionMode = 0;
    private string[] nameActionMode = { "Create new multi-layers material to paint", "Paint with a material" };

    // Material Creation
    enum textureName
    {
        R,
        G,
        B,
        A,
        Bumb_R,
        Bumb_G,
        Bumb_B,
        Bumb_A,
        numMax
    }
    private string matName;
    private Texture[] matTexture = new Texture[(int)textureName.numMax];

    private Material LayerMatToPaint;

    //GUI Variables//
    private bool tgl_Paint;
    private string str_Paint;
    private bool tgl_ShowVertexColors;
    private string str_ShowVertexColors;
    private Color gui_BrushColor;
    private float gui_BrushSize;
    private float gui_BrushOpacity;
    private string gui_Notification;
    private bool canPaint;



    [MenuItem("Tools/Orisha Vertex Painter")]
    private static void OpenWindow()
    {
        currentWindow = EditorWindow.GetWindow<Orisha_VertexPainter>(false, "Orisha Vertex Painter");
        currentWindow.Show();
        currentWindow.Repaint();
    }

    void OnEnable()
    {
        tgl_Paint = true;
        if (SceneView.onSceneGUIDelegate != OnSceneGUI)
            SceneView.onSceneGUIDelegate += OnSceneGUI;

        ResetPlus();
    }
    void OnDisable()
    {
        tgl_Paint = false;
        if (SceneView.onSceneGUIDelegate != OnSceneGUI)
            SceneView.onSceneGUIDelegate -= OnSceneGUI;

        if (newMaterial != null)
            DestroyImmediate(newMaterial);
    }

    void OnSelectionChange()
    {
        ResetPlus();

    }

    void OnProjectChange()
    {
        //ResetPlus();
    }

    private void OnInspectorUpdate()
    {
        this.Repaint();
    }
    private void OnGUI()
    {
        numActionMode = EditorGUILayout.Popup("Action: ", numActionMode, nameActionMode);

        if (numActionMode == 0)
        {
            NewMaterialCreation();
        }
        else if (numActionMode == 1)
        {
            Painting();
        }
    }

    private void NewMaterialCreation()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUI.indentLevel = 1;

        matName = EditorGUILayout.TextField("Material's name", matName);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.indentLevel = 3;
        for (int i = 0; i < (int)textureName.numMax; ++i)
        {
            matTexture[i] = EditorGUILayout.ObjectField("Layer " + ((textureName)i).ToString(), matTexture[i], typeof(Texture), false) as Texture;
        }
        EditorGUI.indentLevel = 0;

        bool valid = (matName != null && matName != string.Empty) ? true : false;

        if (!valid)
        {
            EditorGUILayout.HelpBox("Please, set a name to your new material to save it.", MessageType.Warning);
        }

        GUI.enabled = valid;
        if (GUILayout.Button("Save new mat"))
        {
            string path = EditorUtility.OpenFolderPanel("Choose a destination folder ", "", "");

            Material mat = new Material(Shader.Find("Orisha/VertexPainter/Standard"));


            string finalPath = path.Substring(path.IndexOf("Assets/"));
            string finalName = finalPath + "/" + matName.Replace(' ', '_') + ".mat";

            AssetDatabase.CreateAsset(mat, finalName);

            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();

            string[] layerNames = { "_Splat0", "_Splat1", "_Splat2", "_Splat3", "_Normal0", "_Normal1", "_Normal2", "_Normal3" };

            for (int i = 0; i < (int)textureName.numMax; ++i)
            {
                if (matTexture[i] != null)
                    mat.SetTexture(layerNames[i], matTexture[i]);
            }

            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Material is create", "The material has been created with success!", "Ok");
        }

        GUI.enabled = true;
    }
    private void Painting()
    {
        //Warnings
        if (!canPaint)
        {
            EditorGUILayout.HelpBox(gui_Notification, MessageType.Warning);
            return;
        }
        //We can paint now
    
        LayerMatToPaint = EditorGUILayout.ObjectField("Painting layer material", LayerMatToPaint, typeof(Material), false) as Material;

        if (LayerMatToPaint != null)
        {
            //We can paint now
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(str_Paint))
            {
                tgl_Paint = !tgl_Paint;
                if (tgl_Paint)
                {
                    str_Paint = "STOP PAINTING";
                    //Debug Material
                    renderer.sharedMaterial = LayerMatToPaint;
                    //Other button
                    tgl_ShowVertexColors = true;
                    str_ShowVertexColors = "HIDE VERTEX COLORS";
                }
                else
                {
                    str_Paint = "START PAINTING";
                    ResetMe();
                }
            }

            if (GUILayout.Button(str_ShowVertexColors))
            {
                tgl_ShowVertexColors = !tgl_ShowVertexColors;
                if (tgl_ShowVertexColors)
                {
                    str_ShowVertexColors = "HIDE VERTEX COLORS";
                    //Debug Material
                    renderer.sharedMaterial = newMaterial;
                }
                else
                {
                    str_ShowVertexColors = "SHOW VERTEX COLORS";
                    renderer.sharedMaterial = LayerMatToPaint;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (tgl_Paint)
            {
                //Top
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                gui_BrushSize = EditorGUILayout.Slider("Brush Size :", gui_BrushSize, 0.1f, 10.0f);
                gui_BrushOpacity = EditorGUILayout.Slider("Brush Opacity :", gui_BrushOpacity, 0.0f, 1.0f);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Brush Color :");
                if (GUILayout.Button("R", GUILayout.ExpandWidth(false)))
                {
                    gui_BrushColor = new Color(1f, 0f, 0f, 0f);
                }
                if (GUILayout.Button("G", GUILayout.ExpandWidth(false)))
                {
                    gui_BrushColor = new Color(0f, 1f, 0f, 0f);
                }
                if (GUILayout.Button("B", GUILayout.ExpandWidth(false)))
                {
                    gui_BrushColor = new Color(0f, 0f, 1f, 0f);
                }
                if (GUILayout.Button("A", GUILayout.ExpandWidth(false)))
                {
                    gui_BrushColor = new Color(0f, 0f, 0f, 1f);
                }
                gui_BrushColor = EditorGUILayout.ColorField(gui_BrushColor, GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                //Center
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Vertex Colors :");
                if (GUILayout.Button("R", GUILayout.ExpandWidth(false)))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        newColors[i] = new Color(1f, 0f, 0f, 0f);
                    }
                    mesh.colors = newColors;
                    EditorUtility.SetDirty(go);
                }
                if (GUILayout.Button("G", GUILayout.ExpandWidth(false)))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        newColors[i] = new Color(0f, 1f, 0f, 0f);
                    }
                    mesh.colors = newColors;
                    EditorUtility.SetDirty(go);
                }
                if (GUILayout.Button("B", GUILayout.ExpandWidth(false)))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        newColors[i] = new Color(0f, 0f, 1f, 0f);
                    }
                    mesh.colors = newColors;
                    EditorUtility.SetDirty(go);
                }
                if (GUILayout.Button("A", GUILayout.ExpandWidth(false)))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        newColors[i] = new Color(0f, 0f, 0f, 1f);
                    }
                    mesh.colors = newColors;
                    EditorUtility.SetDirty(go);
                }
                if (GUILayout.Button("RESET"))
                {
                    mesh.colors = originalColors;
                    EditorUtility.SetDirty(go);
                }
                EditorGUILayout.EndHorizontal();
                //Bottom
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("SAVE"))
                {
                    //Create an instance

                    string path = EditorUtility.SaveFilePanel("Choose a destination folder ", "", "", "");

                    if (path == null || path == string.Empty)
                        return;

                    string finalPath = path.Substring(path.IndexOf("Assets/"));
                    if(finalPath.Contains("."))
                    {
                        finalPath.Remove(finalPath.IndexOf("."));
                    }
                    string finalName = finalPath + ".mat";

                    currentMesh = (Mesh)Instantiate(mesh);
                    currentMesh.colors = newColors;

                    mf.sharedMesh = mesh;
                    mf.sharedMesh.colors = originalColors;
                    
                    mf.mesh = currentMesh;
                    renderer.material = LayerMatToPaint;
                    originalMaterial = LayerMatToPaint;
                    EditorUtility.SetDirty(renderer);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    string name = AssetDatabase.GenerateUniqueAssetPath(finalName);

                    AssetDatabase.CreateAsset(currentMesh, name);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();

                    //Selection.activeObject = currentMesh;

                    //mesh = currentMesh;
                    //originalColors = mesh.colors;


                    EditorUtility.DisplayDialog("Mesh is created", "The mesh has been created with success!", "Ok");
                }
                if (GUILayout.Button("Cancel painting"))
                {
                    //Create an instance

                    //mf.sharedMesh = mesh;
                    //mf.sharedMesh.colors = originalColors;

                    //renderer.sharedMaterial = originalMaterial;

                    //Revert original mesh colors
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }


    void OnSceneGUI(SceneView sceneView)
    {
        if (numActionMode == 1)
        {

            if (!tgl_Paint)
            {
                return;
            }

            Event current = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
            RaycastHit hit;
            //Events
            int controlID = GUIUtility.GetControlID(sceneView.GetHashCode(), FocusType.Passive);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.Layout:
                    {
                        if (!tgl_Paint)
                        {
                            return;
                        }
                        HandleUtility.AddDefaultControl(controlID);
                    }
                    break;
                case EventType.MouseDown:
                case EventType.MouseDrag:
                    {
                        if (current.GetTypeForControl(controlID) == EventType.MouseDrag && GUIUtility.hotControl != controlID)
                        {
                            return;
                        }
                        if (current.alt || current.control)
                        {
                            return;
                        }
                        if (current.button != 0)
                        {
                            return;
                        }
                        if (!tgl_Paint)
                        {
                            return;
                        }
                        if (HandleUtility.nearestControl != controlID)
                        {
                            return;
                        }
                        if (current.type == EventType.MouseDown)
                        {
                            GUIUtility.hotControl = controlID;
                        }
                        //Do painting
                        if (Physics.Raycast(ray, out hit, float.MaxValue))
                        {
                            if (hit.transform == go.transform)
                            {
                                Vector3 hitPos = Vector3.Scale(go.transform.InverseTransformPoint(hit.point), go.transform.localScale);
                                for (int i = 0; i < vertices.Length; i++)
                                {
                                    Vector3 vertPos = Vector3.Scale(vertices[i], go.transform.localScale);
                                    float mag = (vertPos - hitPos).magnitude;
                                    if (mag > gui_BrushSize)
                                        continue;
                                    newColors[i] = Color.Lerp(newColors[i], gui_BrushColor, gui_BrushOpacity);
                                }
                                mesh.colors = newColors;
                            }
                        }
                        current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    {
                        if (!tgl_Paint)
                        {
                            return;
                        }
                        if (GUIUtility.hotControl != controlID)
                        {
                            return;
                        }
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
                case EventType.Repaint:
                    {
                        //Draw paint brush
                        if (Physics.Raycast(ray, out hit, float.MaxValue))
                        {
                            if (hit.transform == go.transform)
                            {
                                Handles.color = new Color(gui_BrushColor.r, gui_BrushColor.g, gui_BrushColor.b, 1.0f);
                                Handles.DrawWireDisc(hit.point, hit.normal, gui_BrushSize);
                            }
                        }
                        HandleUtility.Repaint();
                    }
                    break;
            }
        }
    }

    private void ResetLess()
    {
        if (newMaterial != null)
            DestroyImmediate(newMaterial);

        Shader temp = Shader.Find("Orisha/VertexPainter/VertexColors");
        Debug.Log(temp.ToString());
        newMaterial = new Material(temp);
    }
    private void ResetPlus()
    {
        ResetMe();

        if (newMaterial != null)
            DestroyImmediate(newMaterial);

        Shader temp = Shader.Find("Orisha/VertexPainter/VertexColors");
        newMaterial = new Material(temp);
    }
    private void ResetMe()
    {
        //Reset previously worked on object if any
        if (go && originalMaterial)
        {
            go.GetComponent<Renderer>().sharedMaterial = originalMaterial;
            mesh.colors = originalColors;
        }

        //Reset variables
        go = null;
        collider = null;
        mf = null;
        mesh = null;
        currentMesh = null;
        renderer = null;
        vertices = null;
        originalColors = null;
        newColors = null;
        originalMaterial = null;

        //Reset gui variables
        tgl_Paint = false;
        str_Paint = "START PAINTING";
        tgl_ShowVertexColors = false;
        str_ShowVertexColors = "SHOW VERTEX COLORS";
        gui_BrushColor = new Color(1f, 0f, 0f, 0f);
        gui_BrushSize = 1.0f;
        gui_BrushOpacity = 0.5f;
        canPaint = false;

        //Reset Selection
        go = Selection.activeGameObject;
        if (go != null)
        {
            collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                mf = go.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    mesh = mf.sharedMesh;
                    
                    if (mesh != null)
                    {
                        //Save originals
                        renderer = go.GetComponent<Renderer>();
                        originalMaterial = renderer.sharedMaterial;
                        originalColors = mesh.colors;
                        //Set Arrays
                        vertices = mesh.vertices;
                        if (mesh.colors.Length > 0)
                            newColors = mesh.colors;
                        else
                        {
                            Debug.LogWarning("Mesh originally has no vertex color data!!");
                            newColors = new Color[vertices.Length];
                        }
                        //All is okay, we can paint now
                        canPaint = true;
                    }
                    else
                        gui_Notification = "Object doesnt have a mesh!";
                }
                else
                    gui_Notification = "Object doesnt have a MeshFilter!";
            }
            else
                gui_Notification = "Object doesnt have a collider!";
        }
        else
            gui_Notification = "No object selected!";
    }
}

using UnityEngine;
using UnityEditor;

public class PrefabTools : Editor {
    [MenuItem("Tools/Revert to Prefab %r")]
    static void Revert()
    {
        var selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++) {
                PrefabUtility.ResetToPrefabState(selection[i]);
                PrefabUtility.RevertPrefabInstance(selection[i]);
                Debug.Log(selection[i].name);
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }
}

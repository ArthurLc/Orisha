using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSand_Shader : MonoBehaviour
{
    public Material fallingSandShader;
    float depth;
    public bool isTwoSided = true;

    void Start ()
    {
        List<MeshRenderer> meshes = new List<MeshRenderer>();

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            if (mesh.material.shader == fallingSandShader.shader)
                meshes.Add(mesh);
        }

        if (meshes.Count == 0)
            Destroy(this);

        float ratio = isTwoSided ? 2.0f : 1.0f;
        float length = meshes[0].gameObject.transform.localScale.y * meshes.Count / ratio;
        foreach(MeshRenderer mesh in meshes)
        {
            depth = transform.position.y + length;
            mesh.material.SetFloat("_Start", transform.position.y);
            mesh.material.SetFloat("_End", transform.position.y - length);
        }

        Destroy(this);
	}
}

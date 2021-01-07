using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMovement : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    private float angle;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        angle = 0;
    }

    void Update()
    {
        for (var i = 0; i < vertices.Length; i = i + 10)
        {
            vertices[i].z = /*Time.deltaTime*/ 0.001f * Mathf.Cos(angle);
        }
        Debug.Log(vertices[0].z);
        angle++;

        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}

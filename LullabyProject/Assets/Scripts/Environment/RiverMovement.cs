using UnityEngine;
using System.Collections;


// Only the upward movement of the river for the moment

public class RiverMovement : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    float angle;
    int i;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        angle = 0f;
        // invoke the method ModifyZCoordinate every 0.1 seconds
        InvokeRepeating("ModifyZCoordinate", 0f, 0.1f);
    }

    void Update()
    {
    }

    void ModifyZCoordinate()
    {
        // one out of 20 vertices will follow the cosine function
        for (var i = 0; i < vertices.Length; i += 20)
        {
            vertices[i].z = 1f + Mathf.Cos(angle) * 0.1f;
        }
        for (var i = 1; i < vertices.Length; i += 20)
        {
            vertices[i].z = 1f + Mathf.Sin(angle) * 0.1f;
        }
        angle += 0.5f;
        
        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}
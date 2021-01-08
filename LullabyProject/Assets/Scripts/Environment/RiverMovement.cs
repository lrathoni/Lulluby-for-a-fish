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
        // Repeat function every 3 seconds
        InvokeRepeating("ChangeCoordinates", 0f, 0.1f);
    }

    void ChangeCoordinates()
    {
        for (var i = 0; i < vertices.Length; i = i + 2)
        {
            //vertices[i].z = 0.5f +/*Time.deltaTime*/ 0.1f * Mathf.Cos(angle);
        }
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3 (currentPosition.x, currentPosition.y, currentPosition.z + 0.3f * Mathf.Cos(angle));
        //Debug.Log(vertices[0].z);
        angle++;

        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}

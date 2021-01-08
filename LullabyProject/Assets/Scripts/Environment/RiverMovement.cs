using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Up/down movement of the river

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
        // Repeat function every 0.1 seconds
        InvokeRepeating("ChangeCoordinates", 0f, 0.1f);
    }

    // Coordinates follow cosine function
    void ChangeCoordinates()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3 (currentPosition.x, currentPosition.y + 0.05f * Mathf.Cos(angle), currentPosition.z);
        angle++;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isOpen;
    Vector3 tempPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tempPos = transform.position;
        tempPos.y += 0.1f;
        transform.position = tempPos;
    }

    // sqrt((51.63 - 48.8567)²)
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class HandMovement : MonoBehaviour
{

   public GameObject hand;
   public KeyCode[] keys;
   private float[] positionNote = {0.9f, 0.8f, 0.7f, 0.6f, 0.5f};

   public Transform cameraTransform;

   private float distanceFromCamera = 2f;

   private float resetTime = 0f;
   private bool isReset = false;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(keys.Length == positionNote.Length, "The key length must be the same as the different positions of the flute for each note");
    }


    // Update is called once per frame
    void Update()
    {
        
        for (int keyIndex = 0; keyIndex < keys.Length; ++keyIndex)
        {
            if(Input.GetKeyDown(keys[keyIndex]))
            {
                distanceFromCamera = positionNote[keyIndex];
                isReset = true;
                resetTime = 0;
            }
            
        }

        Vector3 resultingPosition = cameraTransform.position + cameraTransform.forward * 0.1f + cameraTransform.right * distanceFromCamera;
        transform.position = resultingPosition;

        if(isReset) // we need to check here if the note is still playing
        {
            if(resetTime <= 3)
            {
                resetTime += Time.deltaTime;
            }
            if(resetTime>3)
            {
                distanceFromCamera = 2f;
                isReset = false;
                resetTime = 0;
            }
        }
        
    }

    
}

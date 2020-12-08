using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note Position : 
// Press Y : 0.816f 1.7f 0.244f 0.9
// Press U : 0.722 1.7 0.263 0.8
// Press I : 0.637 1.7 0.28 0.7
// Press O : 0.565 1.7 0.295 0.6
// Press P : 0.414 1.7 0.326 0.5

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

        if(isReset)
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

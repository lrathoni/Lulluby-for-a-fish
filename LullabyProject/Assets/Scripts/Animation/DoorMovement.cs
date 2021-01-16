using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isOpen;
    float speed = 1f;
    bool moving = false;
    //Vector3 tempPos;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(moving)
        {
            if(isOpen)
            {
                DoorClosed();
            }
            else
            {
                DoorOpen();
            }
            
        }
        //tempPos = transform.position;
        //tempPos.y += 0.1f;
        //transform.position = tempPos;
    }

   public void StartAnimation()
   {
       Debug.Log("début animation");
       moving = true;
   }

   public void StopAnimation()
   {
       Debug.Log("fin animation");
       moving = false;
   }

    public void DoorOpen()
    {
        if(transform.position.y < 53)
        {
            transform.position += new Vector3(0,Time.deltaTime*speed,0);
            Debug.Log("porte s'ouvre");
        }
        else
        {
            isOpen = true;
            StopAnimation();
            Debug.Log("porte est ouverte");
        }
        //isOpen = true;
    }

    public void DoorClosed()
    {
        if(transform.position.y > 49)
        {
            transform.position -= new Vector3(0,Time.deltaTime*speed,0);
            Debug.Log("porte se ferme");
        }
        else
        {
            isOpen = false;
            StopAnimation();
            Debug.Log("porte est fermée");
        }
    }

  

    // sqrt((51.63 - 48.8567)²)
    // Open : y = 53
    // Closed : y = 49 
}

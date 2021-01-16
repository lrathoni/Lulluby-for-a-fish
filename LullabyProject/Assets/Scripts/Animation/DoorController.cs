using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] doors;


   
    // Update is called once per frame

    void Update()
    {
                
        if(Input.GetKeyDown("space"))
        {
            Debug.Log("coucou");
            
            foreach(GameObject door in doors)
            {
                bool state = door.GetComponent<DoorMovement>().isOpen;
                Debug.Log(door);
                Debug.Log(state);
                door.GetComponent<DoorMovement>().StartAnimation();
                /*
                if(state)
                {
                    door.GetComponent<DoorMovement>().DoorClosed();
                }
                
                else
                {
                    door.GetComponent<DoorMovement>().DoorOpen();
                }
                */
            }
            
        
        }
        
        
    }
}

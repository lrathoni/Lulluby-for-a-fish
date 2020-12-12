using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    //public CharacterController controller;
    public NavMeshAgent player;
    public float speed = 1.5f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if(x != 0 || z != 0)
            player.SetDestination(player.transform.position + move);
        x = 0;
        z = 0;

        /*if(isWalkable)
            controller.Move(move * speed * Time.deltaTime);
        */
        
    }
}

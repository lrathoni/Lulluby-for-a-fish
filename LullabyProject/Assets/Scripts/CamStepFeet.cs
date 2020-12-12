/***
 * Script kindly shared by Mcsting22
 * via pastebin.com/n0GiZxc1 
 */

using UnityEngine;
using System.Collections;

public class CamStepFeet : MonoBehaviour
{
    public CharacterController playerController;
    public Animation anim; //Empty GameObject's animation component
    private bool isMoving;

    private bool left;
    private bool right;

    void CameraAnimations()
    {        
        if (isMoving == true)
        {
            if (left == true)
            {
                if (!anim.isPlaying)
                {//Waits until no animation is playing to play the next
                    anim.Play("walkLeft");
                    left = false;
                    right = true;
                    Debug.Log("\tLeft foot");
                }
            }
            if (right == true)
            {
                if (!anim.isPlaying)
                {
                    anim.Play("walkRight");
                    right = false;
                    left = true;
                    Debug.Log("\tRight foot");
                }
            }
        }
    }


    void Start()
    { //First step in a new scene/life/etc. will be "walkLeft"
        left = true;
        right = false;
    }


    void Update()
    {
        float inputX = Input.GetAxis("Horizontal"); //Keyboard input to determine if player is moving
        float inputY = Input.GetAxis("Vertical");

        if (inputX != 0 || inputY != 0)
        {
            isMoving = true;
        }
        else if (inputX == 0 && inputY == 0)
        {
            isMoving = false;
        }

        CameraAnimations();

    }
}
/***
 * Script kindly shared by Mcsting22
 * via pastebin.com/n0GiZxc1 
 */

using UnityEngine;

namespace Behaviour
{
    
public class CamStepFeet : MonoBehaviour
{
    public GameObject playerObject;

    //Empty GameObject's animation component
    public Animation anim;

    bool left;
    bool right;

    void CameraAnimations()
    {
        if (m_playerMovement.IsMoving())
        {
            if (left)
            {
                if (!anim.isPlaying)
                {
                    //Waits until no animation is playing to play the next
                    anim.Play("walkLeft");
                    left = false;
                    right = true;
                    // Debug.Log("\tLeft foot");
                }
            }

            if (right)
            {
                if (!anim.isPlaying)
                {
                    anim.Play("walkRight");
                    right = false;
                    left = true;
                    // Debug.Log("\tRight foot");
                }
            }
        }
    }


    void Start()
    {
        m_playerMovement = playerObject.GetComponent<PlayerMovement>();

        //First step in a new scene/life/etc. will be "walkLeft"
        left = true;
        right = false;
    }


    void Update()
    {
        CameraAnimations();

    }

    PlayerMovement m_playerMovement;
    }
}


using UnityEngine;
using UnityEngine.Assertions;

using Music;

namespace IO.Behaviour
{
    
public class HandMovement : MonoBehaviour
{

    #region Serialised data

    public float[] noteOffsets = {0.9f, 0.8f, 0.7f, 0.6f, 0.5f};

    public float stopDistanceFromCamera = 1.2F;
    public float restDistanceFromCamera = 2.0F;

    public GameObject interfaceObject;
    
    #endregion
    #region Unity Monobehaviour events

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(NoteColours.GetNumber() == noteOffsets.Length,
            "Positions of the notes must correspond to NoteColours.");
            
        m_flutePlayer = interfaceObject.GetComponent<FlutePlayer>();
        Assert.IsNotNull(m_flutePlayer);

        // m_flutePlayer.AddOnStateEnterListener(OnPlayerEnterState);
        
        Camera cam = Camera.main;
        Assert.IsNotNull(cam);
        m_cameraTransform = cam.transform;
        // setting this transform as the child of the camera's
        // so that the movement is the same regardless of camera angle
        gameObject.transform.parent = m_cameraTransform;
        // Move it to rest position
    }

    void OnDestroy()
    {
        // m_flutePlayer.RemoveOnStateEnterListener(OnPlayerEnterState);
    }
    
    #endregion
    #region Movement

    void MoveHand(float dist)
    {
        Vector3 resultingPosition = m_cameraTransform.position 
                                    + m_cameraTransform.forward * 0.1f 
                                    + m_cameraTransform.right * dist;
        transform.position = resultingPosition;
    }

    #endregion
    #region Flute event callbacks

    #endregion
    #region Private data

    // cached, the both of them.
    FlutePlayer m_flutePlayer;
    Transform m_cameraTransform;

    #endregion

}

}

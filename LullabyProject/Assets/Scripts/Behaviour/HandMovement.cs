
using UnityEngine;
using UnityEngine.Assertions;

using MptUnity.IO.Behaviour;

namespace Behaviour
{
    
public class HandMovement : MonoBehaviour
{

    #region Serialised data

    public float[] positionNote = {0.9f, 0.8f, 0.7f, 0.6f, 0.5f};

    public float stopDistanceFromCamera = 1.2F;
    public float restDistanceFromCamera = 2.0F;

    public GameObject interfaceObject;
    
    #endregion
    #region Unity Monobehaviour events

    // Start is called before the first frame update
    void Start()
    {
        m_flutePlayer = interfaceObject.GetComponent<FlutePlayer>();
        Assert.IsNotNull(m_flutePlayer);

        m_flutePlayer.AddOnEnterStateListener(OnPlayerEnterState);
        m_flutePlayer.AddOnReceiveNoteCommandListener(OnPlayerReceiveNoteCommand);
        
        Camera cam = Camera.main;
        Assert.IsNotNull(cam);
        m_cameraTransform = cam.transform;
        // setting this transform as the child of the camera's
        // so that the movement is the same regardless of camera angle
        gameObject.transform.parent = m_cameraTransform;
        // Move it to rest position
        MoveHand(restDistanceFromCamera);
    }

    void OnDestroy()
    {
        m_flutePlayer.RemoveOnEnterStateListener(OnPlayerEnterState);
        m_flutePlayer.RemoveOnReceiveNoteCommandListener(OnPlayerReceiveNoteCommand);
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

    void OnPlayerEnterState(FlutePlayer.EPlayingState state, FlutePlayer player)
    {
        Debug.Log(state.ToString());
        
        switch (state)
        {
           case FlutePlayer.EPlayingState.eStopped: 
                MoveHand(stopDistanceFromCamera);
                break;
           case FlutePlayer.EPlayingState.eResting:
                MoveHand(restDistanceFromCamera);
                break;
        }
        
    }

    void OnPlayerReceiveNoteCommand(FlutePlayer.NoteCommand command)
    {
        // todo: we have to start the animation here because
        // there is no 'Getting ready to play a note' FlutePlayer.EPlayingState yet.
        switch (command.kind)
        {
            case FlutePlayer.NoteCommand.Kind.eStart:
                MoveHand(positionNote[command.toneIndex]);
                break;
        }
    }
    
    #endregion
    #region Private data

    // cached, the both of them.
    FlutePlayer m_flutePlayer;
    Transform m_cameraTransform;
    
    #endregion

}

}

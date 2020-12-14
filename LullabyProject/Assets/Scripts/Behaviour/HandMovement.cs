
using UnityEngine;
using UnityEngine.Assertions;

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
        m_flutePlayer.AddOnNoteCommandReceiveListener(OnPlayerNoteCommandReceive);
        m_flutePlayer.AddOnNoteCommandCancelListener(OnPlayerNoteCommandCancel);
        
        Camera cam = Camera.main;
        Assert.IsNotNull(cam);
        m_cameraTransform = cam.transform;
        // setting this transform as the child of the camera's
        // so that the movement is the same regardless of camera angle
        gameObject.transform.parent = m_cameraTransform;
        // Move it to rest position
        ResetHandState(FlutePlayer.EPlayingState.eResting);
    }

    void OnDestroy()
    {
        m_flutePlayer.RemoveOnEnterStateListener(OnPlayerEnterState);
        m_flutePlayer.RemoveOnNoteCommandReceiveListener(OnPlayerNoteCommandReceive);
        m_flutePlayer.RemoveOnNoteCommandCancelListener(OnPlayerNoteCommandCancel);
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

    void ResetHandState(FlutePlayer.EPlayingState playingState)
    {
        switch (playingState)
        {
           case FlutePlayer.EPlayingState.eStopped: 
                MoveHand(stopDistanceFromCamera);
                break;
           case FlutePlayer.EPlayingState.eResting:
                MoveHand(restDistanceFromCamera);
                break;
        }

        m_previousNotPlayingState = playingState;
    }
    
    #endregion
    #region Flute event callbacks

    void OnPlayerEnterState(FlutePlayer.EPlayingState state, FlutePlayer player)
    {
        ResetHandState(state);
    }

    void OnPlayerNoteCommandReceive(FlutePlayer.NoteCommand command)
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
    
    void OnPlayerNoteCommandCancel(FlutePlayer.NoteCommand command)
    {
        // same thing as in Receive.
        switch (command.kind)
        {
            case FlutePlayer.NoteCommand.Kind.eStart:
                ResetHandState(m_previousNotPlayingState);
                break;
        }
    }
    
    #endregion
    #region Private data

    // cached, the both of them.
    FlutePlayer m_flutePlayer;
    Transform m_cameraTransform;

    FlutePlayer.EPlayingState m_previousNotPlayingState;

    #endregion

}

}

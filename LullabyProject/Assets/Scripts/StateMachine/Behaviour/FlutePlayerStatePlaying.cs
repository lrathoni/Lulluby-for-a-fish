using UnityEngine;

using MptUnity.Audio;
using Music;

namespace StateMachine.Behaviour
{
    public class FlutePlayerStatePlaying : AbstractFlutePlayerState
    {
        #region AbstractFlutePlayerState resolution

        protected override void OnStateEnterInternal(Animator animator)
        {
            // Resetting the corresponding trigger.
            animator.ResetTrigger("TNoteStarted");
            
            owner.StopAllPlayingNotes();

            ENoteColour colour = (ENoteColour) animator.GetInteger("NoteColour");
            
            m_note = new MusicalNote(
                animator.GetInteger("NoteTone"),
                animator.GetFloat("NoteVolume")
            );
            // Debug.Log($"Start {colour}");
            owner.PlayNote(colour, m_note);
        }
        
        #endregion

        #region Private data

        MusicalNote m_note;

        #endregion
    }
}

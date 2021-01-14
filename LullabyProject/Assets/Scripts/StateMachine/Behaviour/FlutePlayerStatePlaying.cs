using UnityEngine;

using MptUnity.Audio;

namespace StateMachine.Behaviour
{
    public class FlutePlayerStatePlaying : AbstractFlutePlayerState
    {
        #region AbstractFlutePlayerState resolution

        protected override void OnStateEnterInternal(Animator animator)
        {
            // Resetting the corresponding trigger.
            animator.ResetTrigger("TNoteStarted");
            
            m_note = new MusicalNote(
                animator.GetInteger("NoteTone"),
                animator.GetFloat("NoteVolume")
            );
            // Debug.Log($"Start {colour}");
            owner.PlayNote(colour, m_note);
        }

        protected override void OnStateExitInternal()
        {
            // Debug.Log($"Stop {colour}");
            owner.StopNote(colour);
        }

        #endregion

        #region Private data

        MusicalNote m_note;

        #endregion
    }
}

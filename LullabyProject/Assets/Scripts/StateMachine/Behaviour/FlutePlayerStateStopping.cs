using UnityEngine;
using Music;

namespace StateMachine.Behaviour
{

    public class FlutePlayerStateStopping : AbstractFlutePlayerState
    {
        #region AbstractFlutePlayerState resolution

        protected override void OnStateEnterInternal(Animator animator)
        {
            // Resetting the corresponding trigger.
            animator.ResetTrigger("TNoteStopped");

            // Debug.Log($"Stop {colour}");
            owner.StopAllPlayingNotPressedNotes();
        }

        #endregion
    }

}
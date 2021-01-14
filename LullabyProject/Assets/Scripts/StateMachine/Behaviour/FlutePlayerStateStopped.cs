using UnityEngine;

namespace StateMachine.Behaviour
{

    public class FlutePlayerStateStopped : AbstractFlutePlayerState
    {
        #region AbstractFlutePlayerState resolution

        protected override void OnStateEnterInternal(Animator animator)
        {
            // Resetting the corresponding trigger.
            animator.ResetTrigger("TNoteStopped");
        }

        #endregion
    }

}
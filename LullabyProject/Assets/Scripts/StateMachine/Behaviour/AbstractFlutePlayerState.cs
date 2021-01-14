
using UnityEngine;
using UnityEngine.Assertions;

using IO.Behaviour;
using Music;

namespace StateMachine.Behaviour
{

    public abstract class AbstractFlutePlayerState : StateMachineBehaviour
    {
        #region Set-up

        public void SetOwner(FlutePlayer aOwner)
        {
            Assert.IsNotNull(aOwner);
            owner = aOwner;
        }

        #endregion
        
        #region StateMachineBehaviour stuff

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Passing up the note colour info from the flute player.
            // Basically circumventing the lack of a constructor.
            colour = (ENoteColour) animator.GetInteger("NoteColour");
            OnStateEnterInternal(animator);
            owner.SignalStateEnterEvent(this);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnStateExitInternal();
            owner.SignalStateExitEvent(this);
        }

        #endregion


        #region To resolve

        // IMPORTANT: Do not call it OnStateEnter, OnStateExit or it will mess with polymorphism.
        // Has to do with how Unity handles bindings (same as with Start etc.).
        protected virtual void OnStateEnterInternal(Animator animator) { }
        protected virtual void OnStateExitInternal() { }

        #endregion

        #region Private data

        protected FlutePlayer owner;
        protected ENoteColour colour;

        #endregion

    }
}

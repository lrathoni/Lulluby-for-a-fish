
using UnityEngine;
using UnityEngine.Assertions;

using IO.Behaviour;

using MptUnity.Audio;
using Music;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An abstract class for objects which should react to the flute playing notes.
    /// </summary>
    public abstract class AbstractMusicInteractiveObject : MonoBehaviour
    {

        #region Unity MonoBehaviour events

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Using polymorphism with MonoBehaviour events requires that they be declared virtual,
        /// since they do not use inheritance.
        /// see: https://stackoverflow.com/a/53085621
        /// </remarks>
        protected virtual void Start()
        {
            var userInterfaceObject = GameObject.FindGameObjectWithTag(c_userInterfaceTag);
            Assert.IsNotNull(userInterfaceObject, "UserInterface object was not found in Scene when starting!");
            m_flutePlayer = userInterfaceObject.GetComponent<FlutePlayer>();
            Assert.IsNotNull(m_flutePlayer);

            m_isSet = true;
            
            Subscribe();
        }

        protected virtual void OnDestroy()
        {
            EndSubscription();
        }

        protected virtual void OnDisable()
        {
            if (m_isSet)
            {
                EndSubscription();
            }
        }

        protected virtual void OnEnable()
        {
            // will be called before Start. Will have to check that the object is set.
            if (m_isSet)
            {
                Subscribe();
            }
        }

        #endregion
        #region To resolve
        
        protected abstract void OnNoteStart(ENoteColour aNoteColour, MusicalNote note);
        protected abstract void OnNoteStop(ENoteColour noteColour, MusicalNote note);

        #endregion
        #region Private utility

        void Subscribe()
        {
            m_flutePlayer.AddOnNoteStartListener(OnNoteStart);
            m_flutePlayer.AddOnNoteStopListener(OnNoteStop);
        }

        void EndSubscription()
        {
            m_flutePlayer.RemoveOnNoteStartListener(OnNoteStart);
            m_flutePlayer.RemoveOnNoteStopListener(OnNoteStop);
        }

        #endregion
        #region Private data
        
        const string c_userInterfaceTag = "UserInterface";
        FlutePlayer m_flutePlayer;

        bool m_isSet;

        #endregion
    }
}
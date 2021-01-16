
using System;
using System.Linq;
using System.Collections.Generic;

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

        #region MonoBehaviour events

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Using polymorphism with MonoBehaviour events requires that they be declared virtual,
        /// since they do not use inheritance.
        /// see: https://stackoverflow.com/a/53085621
        /// </remarks>
        protected virtual void Start()
        {
            var userInterfaceObject = GameObject.FindGameObjectWithTag(Utility.Tags.c_userInterfaceTag);
            Assert.IsNotNull(userInterfaceObject, "UserInterface object was not found in Scene when starting!");
            flutePlayer = userInterfaceObject.GetComponent<FlutePlayer>();
            Assert.IsNotNull(flutePlayer);

            // Does not support adding modifiers at run-time.
            m_rangeModifiers = GetComponents<AbstractInteractiveRangeModifier>();
            
            m_outOfRangePlayingNotes = new List<Tuple<ENoteColour, MusicalNote>>();
            m_inRangePlayingNotes = new List<Tuple<ENoteColour, MusicalNote>>();
            
            m_isSet = true;
            m_isSubscribedInRange = false;
            
            SubscribeOutOfRange();
        }

        protected virtual void Update()
        {
            bool isInRange = IsInRange();
            if (m_isSubscribedInRange && !isInRange)
            {
                UnsubscribeInRange();
                SubscribeOutOfRange();
                
                m_inRangePlayingNotes.ForEach(el => OnNoteStop(el.Item1, el.Item2));
                m_outOfRangePlayingNotes.AddRange(m_inRangePlayingNotes);
                m_inRangePlayingNotes.Clear();
                
            }
            else if (!m_isSubscribedInRange && isInRange)
            {
                UnsubscribeOutOfRange();
                SubscribeInRange();
                
                m_outOfRangePlayingNotes.ForEach(el => OnNoteStart(el.Item1, el.Item2));
                m_inRangePlayingNotes.AddRange(m_outOfRangePlayingNotes);
                m_outOfRangePlayingNotes.Clear();
            }
        }

        protected virtual void OnDestroy()
        {
            EndSubscriptions();
        }

        protected virtual void OnDisable()
        {
            if (m_isSet)
            {
                EndSubscriptions();
            }
        }

        protected virtual void OnEnable()
        {
            // will be called before Start. Will have to check that the object is set.
            if (m_isSet)
            {
                SubscribeOutOfRange();
            }
        }

        #endregion
        
        #region To resolve
        
        protected virtual void OnNoteStart(ENoteColour aNoteColour, MusicalNote note) { }
        protected virtual void OnNoteStop(ENoteColour noteColour, MusicalNote note) { }

        #endregion

        #region Private utility

        void EndSubscriptions()
        {
            if (m_isSubscribedInRange)
            {
                UnsubscribeInRange();
            }
            else
            {
                UnsubscribeOutOfRange();
            }
        }
        void SubscribeInRange()
        {
            flutePlayer.AddOnNoteStartListener(OnNoteStartInRange);
            flutePlayer.AddOnNoteStopListener(OnNoteStopInRange);

            m_isSubscribedInRange = true;
        }

        void UnsubscribeInRange()
        {
            flutePlayer.RemoveOnNoteStartListener(OnNoteStartInRange);
            flutePlayer.RemoveOnNoteStopListener(OnNoteStopInRange);

            m_isSubscribedInRange = false;
        }

        void SubscribeOutOfRange()
        {
            flutePlayer.AddOnNoteStartListener(OnNoteStartOutOfRange);
            flutePlayer.AddOnNoteStopListener(OnNoteStopOutOfRange);
        }

        void UnsubscribeOutOfRange()
        {
            flutePlayer.RemoveOnNoteStartListener(OnNoteStartOutOfRange);
            flutePlayer.RemoveOnNoteStopListener(OnNoteStopOutOfRange);
        }
        
        void OnNoteStartInRange(ENoteColour noteColour, MusicalNote note)
        {
            m_inRangePlayingNotes.Add(new Tuple<ENoteColour, MusicalNote>(noteColour, note));
            OnNoteStart(noteColour, note);
        }
        
        void OnNoteStopInRange(ENoteColour noteColour, MusicalNote note)
        {
            m_inRangePlayingNotes.RemoveAll(el => el.Item1 == noteColour);
            OnNoteStop(noteColour, note);
        }

        void OnNoteStartOutOfRange(ENoteColour noteColour, MusicalNote note)
        {
            m_outOfRangePlayingNotes.Add(new Tuple<ENoteColour, MusicalNote>(noteColour, note));
        }
        
        void OnNoteStopOutOfRange(ENoteColour noteColour, MusicalNote note)
        {
            m_outOfRangePlayingNotes.RemoveAll(el => el.Item1 == noteColour);
        }
        
        bool IsInRange()
        {
            return m_rangeModifiers.All(modifier => modifier.IsInRange(flutePlayer));
        }

        #endregion

        #region Protected data

        protected FlutePlayer flutePlayer;

        #endregion
        
        #region Private data
        
        List<Tuple<ENoteColour, MusicalNote>> m_outOfRangePlayingNotes;
        List<Tuple<ENoteColour, MusicalNote>> m_inRangePlayingNotes;

        AbstractInteractiveRangeModifier[] m_rangeModifiers;
        
        bool m_isSet;
        bool m_isSubscribedInRange;

        #endregion
    }
}
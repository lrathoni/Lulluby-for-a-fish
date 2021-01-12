
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

            // Does not support adding modifiers at run-time.
            m_rangeModifiers = GetComponents<AbstractInteractiveRangeModifier>();
            
            m_outOfRangePlayingNotes = new List<Tuple<ENoteColour, MusicalNote>>();
            m_inRangePlayingNotes = new List<Tuple<ENoteColour, MusicalNote>>();
            
            m_isSet = true;
            
            Subscribe();
        }


        protected virtual void Update()
        {
            // Quick check to see if we can return early.
            if (m_inRangePlayingNotes.Count == 0 && m_outOfRangePlayingNotes.Count == 0)
            {
                return;
            }
            // get all newly in-range and playing notes and call NoteStart event.
            var newlyInRange = m_outOfRangePlayingNotes
                    .FindAll(el => IsInRange(el.Item2));
            newlyInRange.ForEach(el =>
                    OnNoteStartInternal(el.Item1, el.Item2)
                );
            // get all newly out-of-range and playing notes and call NoteStop event.
            var newlyOutOfRange = m_inRangePlayingNotes
                    .FindAll(el => !IsInRange(el.Item2));
            newlyOutOfRange
                .ForEach(el =>
                    OnNoteStopInternal(el.Item1, el.Item2)
                );
            // switch the appropriate notes to the other list.
            m_inRangePlayingNotes.RemoveAll(el => newlyOutOfRange.Contains(el));
            m_outOfRangePlayingNotes.RemoveAll(el => newlyInRange.Contains(el));
            // 
            m_inRangePlayingNotes.AddRange(newlyInRange);
            m_outOfRangePlayingNotes.AddRange(newlyOutOfRange);
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
        
        protected virtual void OnNoteStart(ENoteColour aNoteColour, MusicalNote note) { }
        protected virtual void OnNoteStop(ENoteColour noteColour, MusicalNote note) { }

        #endregion
        #region Private utility

        void Subscribe()
        {
            m_flutePlayer.AddOnNoteStartListener(OnNoteStartInternal);
            m_flutePlayer.AddOnNoteStopListener(OnNoteStopInternal);
        }

        void EndSubscription()
        {
            m_flutePlayer.RemoveOnNoteStartListener(OnNoteStartInternal);
            m_flutePlayer.RemoveOnNoteStopListener(OnNoteStopInternal);
        }

        void OnNoteStartInternal(ENoteColour aNoteColour, MusicalNote note)
        {
            if (IsInRange(note))
            { 
                m_inRangePlayingNotes.Add(new Tuple<ENoteColour, MusicalNote>(aNoteColour, note));
                OnNoteStart(aNoteColour, note);
            }
            else
            {
                m_outOfRangePlayingNotes.Add(new Tuple<ENoteColour, MusicalNote>(aNoteColour, note));
            }
        }

        void OnNoteStopInternal(ENoteColour aNoteColour, MusicalNote note)
        {
            // Remove it from the out-of-range of in-range list.
            m_outOfRangePlayingNotes.RemoveAll(el => el.Item1 == aNoteColour);
            m_inRangePlayingNotes.RemoveAll(el => el.Item1 == aNoteColour);
            OnNoteStop(aNoteColour, note);
        }
        
        bool IsInRange(MusicalNote note)
        {
            return m_rangeModifiers.All(modifier => modifier.IsInRange(m_flutePlayer, note));
        }

        #endregion
        #region Private data
        
        FlutePlayer m_flutePlayer;
        const string c_userInterfaceTag = "UserInterface";

        List<Tuple<ENoteColour, MusicalNote>> m_outOfRangePlayingNotes;
        List<Tuple<ENoteColour, MusicalNote>> m_inRangePlayingNotes;

        AbstractInteractiveRangeModifier[] m_rangeModifiers;
        bool m_isSet;

        #endregion
    }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using MptUnity.Audio.Behaviour;
using MptUnity.Audio;

using Menu;
using Music;
using StateMachine.Behaviour;

namespace IO.Behaviour
{
    /// <summary>
    /// Event which gets called whenever the FlutePlayer starts playing a MusicalNote.
    /// First argument is index of the tone in the FlutePlayer's range of notes. 
    /// </summary>
    public class OnPlayerNoteStartEvent : UnityEvent<ENoteColour, MusicalNote> { }
    /// <summary>
    /// Event which gets called whenever the FlutePlayer stops playing a MusicalNote.
    /// First argument is index of the tone in the FlutePlayer's range of notes.
    /// </summary>
    public class OnPlayerNoteStopEvent : UnityEvent<ENoteColour , MusicalNote> { }
    
    /// <summary>
    ///Can be used to animate objects based on the Flute.
    /// </summary>
    public class OnPlayerStateEnterEvent : UnityEvent<AbstractFlutePlayerState> { }
    /// <summary>
    ///Can be used to animate objects based on the Flute.
    /// </summary>
    public class OnPlayerStateExitEvent : UnityEvent<AbstractFlutePlayerState> { }

    /// <summary>
    /// A class for the Instrument playing component of the Player.
    /// Watch for notes playing with OnPlayerNoteStartEvent and OnPLayerNoteStopEvent.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class FlutePlayer : UnityEngine.MonoBehaviour
    {

        #region Serialised data 

        public GameObject instrumentSourceObject;
        
        public KeyCode[] keys;
        public int[] tones;

        [Range(0L, 1L)] 
        public double volume = 1L;
        
        #endregion

        #region Public utility

        public Transform GetFluteTransform()
        {
            return instrumentSourceObject.transform;
        }

        #endregion

        #region Unity MonoBehaviour events

        void Awake()
        {
            m_events = new Events();
        }

        void Start()
        {
            Assert.IsTrue(NoteColours.GetNumber() == keys.Length && keys.Length == tones.Length,
                "Keys must of the same length, and correspond to NoteColours.");

            SetupAudio();

            SetupState();

            SetupPauseMenu();
        }

        void OnDestroy()
        {
            FreePauseMenu(); 
        }

        void Update()
        {
            for (int toneIndex = 0; toneIndex < tones.Length; ++toneIndex)
            {
                KeyCode key = keys[toneIndex];
                ENoteColour colour = (ENoteColour) toneIndex;
                if (Input.GetKeyDown(key))
                {
                    int tone = tones[toneIndex];
                    TriggerNoteStart(colour, new MusicalNote(tone, volume));
                }
                else if (Input.GetKeyUp(key))
                {
                    TriggerNoteStop(colour);
                }
            }
        }

        #endregion

        #region StateMachine routine

        void SetupState()
        {
            m_animator = GetComponent<Animator>();
            Assert.IsNotNull(m_animator);
            var allStates = m_animator.GetBehaviours<AbstractFlutePlayerState>();
            foreach (var state in allStates)
            {
               state.SetOwner(this);
            }
        }

        public void SignalStateEnterEvent(AbstractFlutePlayerState state)
        {
            m_events.playerStateEnterEvent.Invoke(state);
        }

        public void SignalStateExitEvent(AbstractFlutePlayerState state)
        {
           m_events.playerStateExitEvent.Invoke(state); 
        }

        void TriggerNoteStart(ENoteColour colour, MusicalNote note)
        {
            // Pass info to the StateMachine through the Animator.
            m_animator.SetInteger("NoteColour", (int)colour);
            m_animator.SetInteger("NoteTone", note.tone);
            m_animator.SetFloat("NoteVolume", (float)note.volume);
            m_animator.SetFloat("NotePanning", (float)note.panning);
            // Triggering the transition to the Playing state!
            m_animator.ResetTrigger("TNoteStopped");
            m_animator.SetTrigger("TNoteStarted");
            
            m_pressedColours.Add(colour);
        }

        void TriggerNoteStop(ENoteColour colour)
        {
            m_animator.SetInteger("NoteColour", (int)colour);
            // Triggering the transition to the Stopped state!
            m_animator.ResetTrigger("TNoteStarted");
            m_animator.SetTrigger("TNoteStopped");

            m_pressedColours.Remove(colour);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true on success, false on failure.</returns>
        public bool StopNote(ENoteColour colour)
        {
            int voice = GetPlayingVoice(colour);
            Assert.IsTrue(voice >= 0 && voice < m_instrumentSource.GetNumberVoices());
            MusicalNote note = m_instrumentSource.GetNote(voice);
            bool success = m_instrumentSource.StopNote(voice);
            if (success)
            {
                UnsetPlayingVoice(colour);
                m_events.playerNoteStopEvent.Invoke(colour, note);
            }
            return success;
        }

        public bool PlayNote(ENoteColour colour, MusicalNote note)
        {
            int voice = m_instrumentSource.StartNote(note);
            bool success = voice != -1;
            if (success)
            {
                SetPlayingVoice(colour, voice);
                m_events.playerNoteStartEvent.Invoke(colour, note);
            }
            return success;
        }

        public void StopAllPlayingNotPressedNotes()
        {
            for (int i = 0; i < m_playingColours.Count; ++i)
            {
                ENoteColour colour = m_playingColours[i];
                if (!m_pressedColours.Contains(colour) && StopNote(colour))
                {
                    --i;
                }
            }
        }

        public void StopAllPlayingNotes()
        {
            for (int i = 0; i < m_playingColours.Count; ++i)
            {
                ENoteColour colour = m_playingColours[i];
                if (!StopNote(colour))
                {
                    --i;
                }
            }
        }
        
        #endregion

        #region Pause Menu stuff

        void SetupPauseMenu()
        {
            
            var gameManagerObject = GameObject.FindGameObjectWithTag(Utility.Tags.c_gameManagerTag);
            Assert.IsNotNull(gameManagerObject);
            m_pauseMenu = gameManagerObject.GetComponent<PauseMenu>();
            Assert.IsNotNull(m_pauseMenu);
            
            Subscribe();
        }

        void FreePauseMenu()
        {
            Unsubscribe();
        }

        void OnPauseMenu(bool isPaused)
        {
            if (isPaused)
            {
                m_instrumentSource.Pause();
            }
            else
            {
                m_instrumentSource.Play();
            }
        }
        

        void Subscribe()
        {
            m_pauseMenu.AddOnPauseMenuListener(OnPauseMenu);
        }
        
        void Unsubscribe()
        {
            m_pauseMenu.RemoveOnPauseMenuListener(OnPauseMenu);
        }

        #endregion

        #region Private utility

        void SetupAudio()
        {
            m_instrumentSource = instrumentSourceObject.GetComponent<IInstrumentSource>();
            m_playingVoices = new int[NoteColours.GetNumber()];
            m_playingColours = new List<ENoteColour>();
            m_pressedColours = new List<ENoteColour>();
            for (int i = 0; i < m_playingVoices.Length; ++i)
            {
                m_playingVoices[i] = -1;
            }
            // Force a number of voices
            // Two might yield better results than only one (hopefully).
            if (m_instrumentSource.NumberVoices < 2)
            {
                m_instrumentSource.NumberVoices = 2; 
            }
        }

        int GetPlayingVoice(ENoteColour colour)
        {
            return m_playingVoices[(int) colour];
        }

        void SetPlayingVoice(ENoteColour colour, int voice)
        {
            m_playingVoices[(int) colour] = voice;
            m_playingColours.Add(colour);
        }

        void UnsetPlayingVoice(ENoteColour colour)
        {
            
            m_playingVoices[(int) colour] = -1;
            m_playingColours.Remove(colour);
        }
        

        #endregion

        #region Events

        class Events
        {
            public readonly OnPlayerNoteStartEvent playerNoteStartEvent;
            public readonly OnPlayerNoteStopEvent playerNoteStopEvent;
            public readonly OnPlayerStateEnterEvent playerStateEnterEvent;
            public readonly OnPlayerStateExitEvent playerStateExitEvent;

            public Events()
            {
                playerNoteStartEvent = new OnPlayerNoteStartEvent();
                playerNoteStopEvent  = new OnPlayerNoteStopEvent();
                playerStateEnterEvent = new OnPlayerStateEnterEvent();
                playerStateExitEvent = new OnPlayerStateExitEvent();
            }
        }

        public void AddOnNoteStartListener(UnityAction<ENoteColour, MusicalNote> onNoteStart)
        {
            m_events.playerNoteStartEvent.AddListener(onNoteStart);
        }

        public void RemoveOnNoteStartListener(UnityAction<ENoteColour, MusicalNote> onNoteStart)
        {
            m_events.playerNoteStartEvent.RemoveListener(onNoteStart);
        }

        public void AddOnNoteStopListener(UnityAction<ENoteColour, MusicalNote> onNoteStop)
        {
            m_events.playerNoteStopEvent.AddListener(onNoteStop);
        }

        public void RemoveOnNoteStopListener(UnityAction<ENoteColour, MusicalNote> onNoteStop)
        {
            m_events.playerNoteStopEvent.RemoveListener(onNoteStop);
        }
        
        public void AddOnStateEnterListener(UnityAction<AbstractFlutePlayerState> onStateEnter)
        {
            m_events.playerStateEnterEvent.AddListener(onStateEnter);
        }
        
        public void RemoveOnStateEnterListener(UnityAction<AbstractFlutePlayerState> onStateEnter)
        {
            m_events.playerStateEnterEvent.RemoveListener(onStateEnter);
        }
        
        public void AddOnStateLeaveListener(UnityAction<AbstractFlutePlayerState> onStateLeave)
        {
            m_events.playerStateExitEvent.AddListener(onStateLeave);
        }
        
        public void RemoveOnStateLeaveListener(UnityAction<AbstractFlutePlayerState> onStateLeave)
        {
            m_events.playerStateExitEvent.RemoveListener(onStateLeave);
        }
        
        #endregion

        #region Private data 

        IInstrumentSource m_instrumentSource;

        AbstractFlutePlayerState m_state;
        
        int[] m_playingVoices;
        List<ENoteColour> m_playingColours;
        List<ENoteColour> m_pressedColours;

        Events m_events;
        Animator m_animator;
        PauseMenu m_pauseMenu;

        #endregion
    }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using MptUnity.Audio.Behaviour;
using MusicalNote = MptUnity.Audio.MusicalNote;

namespace MptUnity.IO.Behaviour
{
    /// <summary>
    /// Event which gets called whenever the FlutePlayer starts playing a MusicalNote.
    /// First argument is index of the tone in the FlutePlayer's range of notes. 
    /// </summary>
    public class OnPlayerNoteStartEvent : UnityEvent<int, MusicalNote> { }
    /// <summary>
    /// Event which gets called whenever the FlutePlayer stops playing a MusicalNote.
    /// First argument is index of the tone in the FlutePlayer's range of notes.
    /// </summary>
    public class OnPlayerNoteStopEvent : UnityEvent<int , MusicalNote> { }
    
    /// <summary>
    /// Event which gets trigger immediately when the FlutePlayer receives a NoteCommand.
    /// Warning: the note may be invalid.
    /// Can be used to animate the Flute.
    /// First argument is index of the tone, second is the delay before it might be stopped.
    /// </summary>
    public class OnPlayerReceiveNoteCommandEvent : UnityEvent<FlutePlayer.NoteCommand> {}
    
    /// <summary>
    ///Can be used to animate the Flute.
    /// </summary>
    public class OnPlayerEnterStateEvent : UnityEvent<FlutePlayer.EPlayingState, FlutePlayer> { }

    /// <summary>
    /// A class for the Instrument playing component of the Player.
    /// Watch for notes playing with OnPlayerNoteStartEvent and OnPLayerNoteStopEvent.
    /// </summary>
    public class FlutePlayer : UnityEngine.MonoBehaviour
    {

        #region Serialised data 

        public GameObject instrumentSourceObject;
        
        public KeyCode[] keys;
        public int[] tones;

        /// <summary>
        /// Delay before starting playing the Flute.
        /// </summary>
        public float delayStart = 0.5F;
        /// <summary>
        /// Delay before switching notes while playing.
        /// </summary>
        public float delaySwitch = 0.0F;
        /// <summary>
        /// Delay before stopping playing the note.
        /// </summary>
        public float delayStop = 0.2F;

        /// <summary>
        /// Delay before 'Resting' state.
        /// </summary>
        public float delayRest = 1.0F;

        [Range(0L, 1L)] 
        public double volume = 1L;
        
        #endregion

        #region State
        
        public enum EPlayingState
        {
            ePlaying,
            eStopped,
            eGettingReady, // todo: should free us from using NoteCommand event for animations.
            eResting
        }

        EPlayingState State
        {
            get => m_state;
            set
            {
                if (value == m_state)
                {
                    return;
                }
                m_state = value;
                if (m_state == EPlayingState.eStopped)
                {
                    m_timeStopped = Time.time;
                }
                // Notify!
                m_events.playerEnterStateEvent.Invoke(m_state, this);
            }
        }

        #endregion

        #region Unity MonoBehaviour events

        void Awake()
        {
            m_events = new Events();

            m_noteCommandQueue = new List<NoteCommand>();
        }

        void Start()
        {
            Assert.IsTrue(keys.Length == tones.Length, 
                "Keys must of the same length, and correspond to tones.");

            StopVoice();            
            State = EPlayingState.eResting;
            
            SetupAudio();
        }

        void Update()
        {
            UpdateInput();
            
            UpdateCommandQueue();

            UpdateState();
        }

        #endregion

        #region Update routine

        void UpdateInput()
        {
            for (int toneIndex = 0; toneIndex < tones.Length; ++toneIndex)
            {
                KeyCode key = keys[toneIndex];
                if (Input.GetKeyDown(key))
                {
                    AddNoteCommand(new NoteStartDelayedCommand(this, toneIndex, Time.time));
                }
                else if (Input.GetKeyUp(key))
                {
                    AddNoteCommand(new NoteStopDelayedCommand(this, toneIndex, Time.time));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Requires that m_noteCommandQueue be sorted by ascending time of issuing.</remarks>
        void UpdateCommandQueue()
        {
            for (int i = 0; i < m_noteCommandQueue.Count;)
            {
                if (m_noteCommandQueue[i].ShouldExecute(Time.time))
                {
                    var command = m_noteCommandQueue[i];
                    m_noteCommandQueue.RemoveAt(i);
                    command.Execute();
                }
                else
                {
                    ++i;
                }
            }
        }

        void UpdateState()
        {
            if (State == EPlayingState.eStopped && Time.time > m_timeStopped + delayRest)
            {
                State = EPlayingState.eResting;
            }
        }

        #endregion

        #region Command queue operations

        void AddNoteCommand(NoteCommand command)
        {
            // For now only one NoteStart command in the command queue at a time.
            if (command.kind == NoteCommand.Kind.eStart)
            {
                m_noteCommandQueue.RemoveAll(noteCommand => noteCommand.kind == NoteCommand.Kind.eStart);
            }

            m_noteCommandQueue.Add(command);
            m_events.playerReceiveNoteCommandEvent.Invoke(command);            
        }

        #endregion

        #region Playing routine

        bool StopNoteNow(int toneIndex)
        {
            if (m_playingVoice == -1 || m_playingToneIndex != toneIndex)
            {
                // failure.
                return false;
            }
            // Get the note BEFORE stopping it!
            MusicalNote note = m_instrumentSource.GetNote(m_playingVoice);

            bool success = StopNoteNowSilent(toneIndex);
            Assert.IsTrue(success, "Call to StopNoteNow should not fail at this point.");
            
            // resetting voice and toneIndex.
            StopVoice();
            State = EPlayingState.eStopped;
            
            // Notifying the listeners that the note just stopped.
            m_events.playerNoteStopEvent.Invoke(toneIndex, note);
            
            return true;
        }

        /// <summary>
        /// Stop note without changing the state or invoking NoteStop event.
        /// To be used to switch between two playing notes. 
        /// </summary>
        /// <param name="toneIndex"></param>
        /// <returns></returns>
        /// <remarks> </remarks>
        bool StopNoteNowSilent(int toneIndex)
        {
            if (m_playingVoice == -1 || m_playingToneIndex != toneIndex)
            {
                return false;
            }
            return m_instrumentSource.StopNote(m_playingVoice);
        }

        int PlayNoteNow(int toneIndex)
        {
            
            // We can't have multiple tones playing in the same voice.
            // We do that silently, since there should not be
            // any change if it is already playing.
            StopNoteNowSilent(m_playingToneIndex);
            //
            int tone = tones[toneIndex];
            //
            int voice = m_instrumentSource.PlayNote(new MusicalNote(tone, volume));

            if (voice == -1)
            {
                // failure.
                return voice;
            }

            m_playingVoice = voice;
            m_playingToneIndex = toneIndex;
            State = EPlayingState.ePlaying;

            // Notifying the listeners that a note is being played.
            m_events.playerNoteStartEvent.Invoke(toneIndex, m_instrumentSource.GetNote(voice));
            
            return voice;
        }

        #endregion

        #region Private utility

        void StopVoice()
        {
            // resetting the playing voice and toneIndex;
            m_playingVoice = -1;
            m_playingToneIndex = -1;
        }
        
        void SetupAudio()
        {
            m_instrumentSource = instrumentSourceObject.GetComponent<IInstrumentSource>();
            
            // Force a number of voices
            // Two might yield better results than only one (hopefully).
            if (m_instrumentSource.NumberVoices < 2)
            {
                m_instrumentSource.NumberVoices = 2; 
            }
        }
        

        #endregion

        #region Events

        class Events
        {
            public readonly OnPlayerNoteStartEvent playerNoteStartEvent;
            public readonly OnPlayerNoteStopEvent playerNoteStopEvent;
            public readonly OnPlayerReceiveNoteCommandEvent playerReceiveNoteCommandEvent;
            public readonly OnPlayerEnterStateEvent playerEnterStateEvent;

            public Events()
            {
                playerNoteStartEvent = new OnPlayerNoteStartEvent();
                playerNoteStopEvent  = new OnPlayerNoteStopEvent();
                playerReceiveNoteCommandEvent  = new OnPlayerReceiveNoteCommandEvent();
                playerEnterStateEvent = new OnPlayerEnterStateEvent();
            }
        }

        public void AddOnNoteStartListener(UnityAction<int, MusicalNote> onNoteStart)
        {
            m_events.playerNoteStartEvent.AddListener(onNoteStart);
        }

        public void RemoveOnNoteStartListener(UnityAction<int, MusicalNote> onNoteStart)
        {
            m_events.playerNoteStartEvent.RemoveListener(onNoteStart);
        }

        public void AddOnNoteStopListener(UnityAction<int, MusicalNote> onNoteStop)
        {
            m_events.playerNoteStopEvent.AddListener(onNoteStop);
        }

        public void RemoveOnNoteStopListener(UnityAction<int, MusicalNote> onNoteStop)
        {
            m_events.playerNoteStopEvent.RemoveListener(onNoteStop);
        }
        
        public void AddOnReceiveNoteCommandListener(UnityAction<NoteCommand> onNoteCommandReceive)
        {
            m_events.playerReceiveNoteCommandEvent.AddListener(onNoteCommandReceive);
        }

        public void RemoveOnReceiveNoteCommandListener(UnityAction<NoteCommand> onNoteCommandReceive)
        {
            m_events.playerReceiveNoteCommandEvent.RemoveListener(onNoteCommandReceive);
        }

        public void AddOnEnterStateListener(UnityAction<EPlayingState, FlutePlayer> onEnterState)
        {
            m_events.playerEnterStateEvent.AddListener(onEnterState);
        }
        
        public void RemoveOnEnterStateListener(UnityAction<EPlayingState, FlutePlayer> onEnterState)
        {
            m_events.playerEnterStateEvent.RemoveListener(onEnterState);
        }
        
        #endregion

        #region Note Commands

        public abstract class NoteCommand
        {
            public enum Kind
            {
                eStart,
                eStop
            }
            protected NoteCommand(FlutePlayer a_owner, int a_toneIndex, Kind a_kind, float a_timeIssued)
            {
                owner = a_owner;
                toneIndex = a_toneIndex;
                timeIssued = a_timeIssued;
                kind = a_kind;
            }
            
            public abstract void Execute();

            /// <summary>
            /// Should the command be executed, now that it is next in line?
            /// </summary>
            /// <returns></returns>
            public bool ShouldExecute(float currentTime)
            {
                return currentTime > GetExecutionTime();
            }

            /// <summary>
            /// Time at which the command should be executed.
            /// Should (probably) only be used for ordering CommandQueue.
            /// </summary>
            /// <returns>Time at which the command should executed</returns>
            public abstract float GetExecutionTime();

            public readonly Kind kind;
            public readonly float timeIssued;
            public readonly int toneIndex;

            protected readonly FlutePlayer owner;
        }

        class NoteStartCommand : NoteCommand
        {
            protected NoteStartCommand(FlutePlayer owner, int toneIndex, float timeIssued) 
                : base(owner, toneIndex, Kind.eStart, timeIssued)
            {
                
            }

            public override float GetExecutionTime()
            {
                // exactly when it was issued.
                return timeIssued;
            }

            public override void Execute()
            {
                int voice = owner.PlayNoteNow(toneIndex);
                if (voice != -1)
                {
                    // UnityEngine.Debug.Log($"{voice} Started playing!");
                }
            }

        }

        class NoteStopCommand : NoteCommand
        {
            protected NoteStopCommand(FlutePlayer owner, int toneIndex, float timeIssued) 
                : base(owner, toneIndex, Kind.eStop, timeIssued)
            {
                
            }
            public override float GetExecutionTime()
            {
                return timeIssued;
            }

            public override void Execute()
            {
                bool success = owner.StopNoteNow(toneIndex);
                if (success)
                {
                    // UnityEngine.Debug.Log($"Stopped playing!");
                }
            }
        }

        class NoteStartDelayedCommand : NoteStartCommand
        {

            public NoteStartDelayedCommand(FlutePlayer owner, int toneIndex, float timeIssued)
                : base(owner, toneIndex, timeIssued)
            {
            }
            public override float GetExecutionTime()
            {
                float executionTime = base.GetExecutionTime();
                switch (owner.State)
                {
                    case EPlayingState.ePlaying: 
                    case EPlayingState.eStopped: executionTime += owner.delaySwitch; break;
                    case EPlayingState.eResting: executionTime += owner.delayStart; break;
                }
                return executionTime;
            }

        }

        class NoteStopDelayedCommand : NoteStopCommand
        {
            public NoteStopDelayedCommand(FlutePlayer owner, int toneIndex, float timeIssued)
                : base(owner, toneIndex, timeIssued)
            {
            }

            public override float GetExecutionTime()
            {
                return base.GetExecutionTime() + owner.delayStop;
            }

        }
        #endregion

        #region Private data 

        IInstrumentSource m_instrumentSource;

        int m_playingToneIndex;
        int m_playingVoice;
        
        Events m_events;

        List<NoteCommand> m_noteCommandQueue;

        EPlayingState m_state;
        float m_timeStopped;

        #endregion
    }
}
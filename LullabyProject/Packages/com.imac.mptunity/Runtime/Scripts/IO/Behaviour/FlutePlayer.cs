
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using MptUnity.Utility;

using MptUnity.Audio.Behaviour;
using MusicalNote = MptUnity.Audio.MusicalNote;

namespace MptUnity.IO.Behaviour
{
    /// <summary>
    /// Event which gets called whenever the FlutePlayer starts playing a MusicalNote.
    /// first argument is index of the tone in the FlutePlayer's range of notes. 
    /// </summary>
    public class OnPlayerNoteStartEvent : UnityEvent<int, MusicalNote> { }
    /// <summary>
    /// Event which gets called whenever the FlutePlayer stops playing a MusicalNote.
    /// first argument is index of the tone in the FlutePlayer's range of notes. 
    /// </summary>
    public class OnPlayerNoteStopEvent : UnityEvent<int , MusicalNote> { }
    
    /// <summary>
    /// A class for the Instrument playing component of the Player.
    /// Watch for notes playing with OnPlayerNoteStartEvent and OnPLayerNoteStopEvent.
    /// </summary>
    public class FlutePlayer : UnityEngine.MonoBehaviour
    {

        #region Serialised data 

        public GameObject instrumentSourceObject;
        
        public int numberVoices = 1;
        public KeyCode[] keys;
        public int[] tones;

        [Range(0L, 1L)] 
        public double volume = 1L;
        
        #endregion

        #region Unity MonoBehaviour events

        void Awake()
        {
            m_events = new Events();
        }

        void Start()
        {
            Assert.IsTrue(keys.Length == tones.Length, 
                "Keys must of the same length, and correspond to tones.");
            Assert.IsTrue(numberVoices <= keys.Length,
                "The maximum number of playing voices should not exceed the number of keys.");

            SetupVoices();            
            
            SetupAudio();
        }

        void Update()
        {
            for (int toneIndex = 0; toneIndex < keys.Length; ++toneIndex)
            {
                KeyCode key = keys[toneIndex];
                int voice;
                if (Input.GetKeyDown(key))
                {
                    voice = PlayNote(toneIndex);
                    if (voice != -1)
                    {
                        // UnityEngine.Debug.Log($"{voice} Started playing!");
                    }
                }
                else if (Input.GetKeyUp(key))
                {
                    voice = StopNote(toneIndex);
                    if (voice != -1)
                    {
                        // UnityEngine.Debug.Log($"{voice} Started playing!");
                    }
                }
            }
        }
        
        #endregion

        #region Playing routine

        int StopNote(int toneIndex)
        {
            int voiceIndex = m_toneToVoiceIndices[toneIndex];

            if (voiceIndex == -1)
            {
                return -1;
            }
            Debug.Log($"toneIndex: {toneIndex}, voiceIndex: {voiceIndex}");
            return StopVoice(voiceIndex);
        }

        int PlayNote(int toneIndex)
        {
            int voiceIndex = ChooseVoiceIndex(toneIndex);
            Debug.Log($"toneIndex: {toneIndex}, voiceIndex: {voiceIndex}");
            if (voiceIndex == -1)
            {
                // invalid, we ignore this note.
                return -1;
            }
            
            int tone = tones[toneIndex];
            int previousVoice = m_playingVoices[voiceIndex];
            
            // We can't have multiple tones playing in the same voice.
            StopVoice(voiceIndex);
            //
            m_toneToVoiceIndices[toneIndex] = voiceIndex;
            m_voiceToToneIndices[voiceIndex] = toneIndex;
            int voice = m_instrumentSource.PlayNote(new MusicalNote(tone, volume));

            m_playingVoices[voiceIndex] = voice;

            // incrementing playing time for all
            for (int i = 0; i < m_playingTimes.Length; ++i)
            {
                ++m_playingTimes[i];
            }
            // and resetting new note time.
            m_playingTimes[voiceIndex] = 0;
            
            // Notifying the listeners that a note is being played.
            m_events.playerNoteStartEvent.Invoke(toneIndex, m_instrumentSource.GetNote(voice));
            
            return voice;
        }

        int StopVoice(int voiceIndex)
        {
            int voice = m_playingVoices[voiceIndex];
            int toneIndex = m_voiceToToneIndices[voiceIndex];

            if (toneIndex == -1)
            {
                return -1;
            }

            if (voice != -1)
            {
                m_instrumentSource.StopNote(voice);
                m_playingVoices[voiceIndex] = -1;
                m_playingTimes[voiceIndex] = 0;
                m_voiceToToneIndices[voiceIndex] = -1;
                m_toneToVoiceIndices[toneIndex]  = -1;
                // Notifying the listeners that the note just stopped.
                m_events.playerNoteStopEvent.Invoke(toneIndex, m_instrumentSource.GetNote(voice));
            }

            return voice;
        }
        
        
        #endregion

        #region Private utility

        int ChooseVoiceIndex(int toneIndex)
        {
            int tone = tones[toneIndex];
            int voiceIndex = -1;
            bool shouldSteal = true;
            bool isValid = true;
            for (int i = 0; i < m_playingVoices.Length; ++i)
            {
                int voice = m_playingVoices[i];
                if (voice == -1)
                {
                    voiceIndex = i;
                    shouldSteal = false;
                }
                else if (tone == m_instrumentSource.GetNote(voice).tone)
                {
                   // if there already is a note with the same tone,
                   // we ignore this command.
                   isValid = false;
                }
            }

            if (shouldSteal)
            {
                // taking ye oldest note.
                voiceIndex = m_playingTimes.GetIndexMax();
            }
            return isValid ? voiceIndex : -1;
        }

        void SetupVoices()
        {
            m_playingVoices = new int[numberVoices];
            m_playingVoices.Fill(-1);
            m_playingTimes = new int[numberVoices];
            m_playingTimes.Fill(0);
            
            m_toneToVoiceIndices = new Dictionary<int, int>(tones.Length);
            m_voiceToToneIndices  = new Dictionary<int, int>(tones.Length);
            for (int i = 0; i < tones.Length; ++i)
            {
                m_toneToVoiceIndices.Add(i, -1);
                m_voiceToToneIndices.Add(i, -1);
            }
        }
        void SetupAudio()
        {
            m_instrumentSource = instrumentSourceObject.GetComponent<IInstrumentSource>();
            
            // Force a number of voices
            m_instrumentSource.NumberVoices = numberVoices; 
        }
        

        #endregion

        #region Events

        class Events
        {
            public readonly OnPlayerNoteStartEvent playerNoteStartEvent;
            public readonly OnPlayerNoteStopEvent playerNoteStopEvent;

            public Events()
            {
                playerNoteStartEvent = new OnPlayerNoteStartEvent();
                playerNoteStopEvent  = new OnPlayerNoteStopEvent();
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

        #endregion

        #region Private data 

        IInstrumentSource m_instrumentSource;
        Dictionary<int, int> m_toneToVoiceIndices;
        Dictionary<int, int> m_voiceToToneIndices;
        int[] m_playingVoices;
        int[] m_playingTimes;

        Events m_events;
        
        #endregion
    }
}
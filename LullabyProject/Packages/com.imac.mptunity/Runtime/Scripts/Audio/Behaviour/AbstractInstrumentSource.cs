
using UnityEngine.Events;

namespace MptUnity.Audio.Behaviour
{
    using InstrumentFile = UnityEngine.TextAsset;

    public abstract class AbstractInstrumentSource
        : UnityEngine.MonoBehaviour, IInstrumentSource
    {

        #region Serialiesd

        public InstrumentFile file;
        public int startingNumberVoices;

        #endregion

        #region Unity Audio management

        void Awake()
        {
            m_source = GetComponent<UnityEngine.AudioSource>();
            // In order to use our procedural filter as the input to the AudioSource,
            // we need to detach any potential AudioClip from it.
            // see: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnAudioFilterRead.html
            m_source.clip = null;

            // must loop over buffer
            m_source.loop = true;

            m_sampleRate = MusicConfig.GetSampleRate();

            Load(file, startingNumberVoices);
        }

        void Update()
        {
            m_sampleRate = MusicConfig.GetSampleRate();
        }

        #endregion

        #region Life-cycle 

        protected AbstractInstrumentSource()
        {
            m_events = new Events();
            m_isReady = false;
        }

        #endregion

        #region IInstrumentSource implementation

        public void StopNote(int voice)
        {
            m_instrument.StopNote(voice);
        }

        public int PlayNote(MusicalNote note)
        {
            int voice = m_instrument.PlayNote(note);
            if (voice == -1)
            {
                UnityEngine.Debug.Log("Failed to play note, err: "
                                      + m_instrument.GetLastErrorMessage());
            }

            return voice;
        }

        public MusicalNote GetNote(int voice)
        {
            return m_instrument.GetNote(voice);
        }

        public int GetSpeed()
        {
            return m_instrument.GetSpeed();
        }

        public int GetCurrentRow()
        {
            return m_instrument.GetCurrentRow();
        }
        
        public int NumberVoices
        {
            get => m_instrument.GetNumberVoices();
            set
            {
                if (value != NumberVoices)
                {
                    m_instrument.SetNumberVoices(value);
                }
            }
        }


        #endregion

        #region To resolve

        protected abstract IInstrument CreateInstrument(byte[] data, int numberVoices);

        public abstract void OnAudioFilterRead(float[] data, int channels);
        
    #endregion

        #region Event handling

        protected class Events
        {
            public readonly OnInstrumentNoteStartEvent instrumentNoteStartEvent;
            public readonly OnInstrumentNoteStopEvent instrumentNoteStopEvent;

            public Events()
            {
                instrumentNoteStartEvent = new OnInstrumentNoteStartEvent();
                instrumentNoteStopEvent  = new OnInstrumentNoteStopEvent();
            }
        }

        public void AddOnNoteStartListener(UnityAction<MusicalNote> onNoteStart)
        {
            m_events.instrumentNoteStartEvent.AddListener(onNoteStart);
        }

        public void RemoveOnNoteStartListener(UnityAction<MusicalNote> onNoteStart)
        {
            m_events.instrumentNoteStartEvent.RemoveListener(onNoteStart);
        }

        public void AddOnNoteStopListener(UnityAction<MusicalNote> onNoteStop)
        {
            m_events.instrumentNoteStopEvent.AddListener(onNoteStop);
        }

        public void RemoveOnNoteStopListener(UnityAction<MusicalNote> onNoteStop)
        {
            m_events.instrumentNoteStopEvent.RemoveListener(onNoteStop);
        }
        #endregion

        #region Utility
        void Load(InstrumentFile instrumentFile, int numberVoices)
        {
            try
            {
                m_instrument = CreateInstrument(instrumentFile.bytes, numberVoices);
                m_isReady = true;
                // Play it continuously for now.
                // todo
                m_source.Play();
            }
            catch (System.ArgumentException)
            {
                UnityEngine.Debug.LogError(
                    "Failed to load MOD instrument."
                    ) ;
            }
        }

        protected bool IsReady()
        {
            return m_isReady;
        }

        #endregion
        #region Protected data

        
        protected IInstrument m_instrument;
        
        protected int m_sampleRate;

        protected readonly Events m_events;

        #endregion
        #region Private data

        UnityEngine.AudioClip m_clip;
        UnityEngine.AudioSource m_source;
        
        bool m_isReady;

        #endregion
    }
}
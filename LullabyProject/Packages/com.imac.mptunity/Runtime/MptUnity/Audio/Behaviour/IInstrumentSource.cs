
using UnityEngine.Events;

namespace MptUnity.Audio.Behaviour
{
    /// <summary>
    /// Event which gets called whenever the InstrumentSource starts playing a MusicalNote.
    /// </summary>
    public class OnInstrumentNoteStartEvent : UnityEvent<MusicalNote> { }
    /// <summary>
    /// Event which gets called whenever the InstrumentSource stops playing a MusicalNote.
    /// </summary>
    public class OnInstrumentNoteStopEvent : UnityEvent<MusicalNote> { }
    
    public interface IInstrumentSource : IAudioSource, IMusicSource // todo: add IPlayable
    {
        #region Playing
        bool StopNote(int voice);
        /// <summary>
        /// Plays the MusicalNote on the instrument.
        /// The instrument will keep playing until StopNote is called.
        /// </summary>
        /// <param name="note"></param>
        /// <returns>voice of the note, -1 on failure. </returns>
        int StartNote(MusicalNote note);

        bool CanStop(int voice);
        bool CanStart(MusicalNote note);

        int NumberVoices { get; set; }

        #endregion


        #region Playback info
        int GetSpeed();

        int GetCurrentRow();
        /// <summary>
        /// Get note currently playing at voice.
        /// </summary>
        /// <param name="voice">In [0, numberVoice-1]</param>
        /// <returns></returns>
        MusicalNote GetNote(int voice);

        int GetNumberVoices();
        #endregion

        #region Events
        void AddOnNoteStartListener(UnityAction<MusicalNote> onNoteStart);
        void RemoveOnNoteStartListener(UnityAction<MusicalNote> onNoteStart);
        
        void AddOnNoteStopListener(UnityAction<MusicalNote> onNoteStop);
        void RemoveOnNoteStopListener(UnityAction<MusicalNote> onNoteStop);
        #endregion
    }
}

using MptUnity.Audio;
using Music;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object which reacts to only one note.
    /// </summary>
    public abstract class AbstractSingleNoteObject : AbstractMusicInteractiveObject
    {
        #region Serialised data

        public ENoteColour noteColour;

        #endregion
        #region To resolve

        
        protected abstract void Activate(MusicalNote note);
        protected abstract void Deactivate(MusicalNote note);

        #endregion

        #region AbstractMusicInteractiveObject resolution

        protected sealed override void OnNoteStart(ENoteColour aNoteColour, MusicalNote note)
        {
            if (aNoteColour == noteColour)
            {
                Activate(note);
            }
        }

        protected sealed override void OnNoteStop(ENoteColour aNoteColour, MusicalNote note)
        {
            if (aNoteColour == noteColour)
            {
                Deactivate(note);
            }
        }

        #endregion
    }
}
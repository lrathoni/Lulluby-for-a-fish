
using MptUnity.Audio;
using Music;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object which reacts to only one note.
    /// </summary>
    public abstract class AbstractSingleNoteInteractiveObject : AbstractMusicInteractiveObject
    {
        #region Serialised data

        public ENoteColour noteColour;

        #endregion
        #region Unity MonoBehaviour events

        protected override void Update()
        {
            base.Update();
            if (m_isActive)
            {
                UpdateActive();
            }
        }

        void FixedUpdate()
        {
            if (m_isActive)
            {
                FixedUpdateActive();
            }
        }

        #endregion
        #region To resolve
        
        protected virtual void Activate(MusicalNote note) { }
        protected virtual void Deactivate(MusicalNote note) { }

        protected virtual void UpdateActive() { }
        protected virtual void FixedUpdateActive() { }

        #endregion
        #region AbstractMusicInteractiveObject resolution

        protected sealed override void OnNoteStart(ENoteColour aNoteColour, MusicalNote note)
        {
            if (aNoteColour == noteColour)
            {
                Activate(note);
                m_isActive = true;
            }
        }

        protected sealed override void OnNoteStop(ENoteColour aNoteColour, MusicalNote note)
        {
            if (aNoteColour == noteColour)
            {
                Deactivate(note);
                m_isActive = false;
            }
        }

        #endregion
        #region Private data

        bool m_isActive;

        #endregion
    }
}
using UnityEngine;

namespace Interaction.Behaviour
{
    public class GlimmerInteractiveObject : AbstractSingleNoteInteractiveObject
    {
        #region Serialised data

        [Range(0.1f, 10.0f)]
        public float activateIntensity = 3.0f;

        #endregion
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();
            
            m_light = gameObject.AddComponent<Light>();
            m_light.color = Music.NoteColours.GetColour(noteColour);
            
            Deactivate(null);
        }

        #endregion
        #region AbstractSingleNoteInteractiveObject

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            m_light.intensity = activateIntensity;
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            m_light.intensity = 0.0f;
        }
        
        #endregion
        #region Private data

        Light m_light;

        #endregion
    }
}
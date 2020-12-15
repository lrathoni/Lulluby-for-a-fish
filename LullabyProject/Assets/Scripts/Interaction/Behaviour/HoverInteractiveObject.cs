
using UnityEngine;
using UnityEngine.Assertions;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that hovers and lights up when activated.
    /// Uses physics to animate and lighting.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class HoverInteractiveObject : AbstractSingleNoteInteractiveObject
    {

        #region Serialised data

        [Range(0.1f, 10.0f)]
        public float activateIntensity = 3.0f;

        public Vector3 hoverDir = Vector3.up;
        [Range(0.1f, 10.0f)]
        public float   hoverStrength = 3.0f;

        #endregion
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();
            SetupLight();


            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);

            hoverDir = hoverDir.normalized;
            
            Deactivate();
        }

        void Update()
        {
            if (m_isActive)
            {
                
            }
        }

        #endregion
        #region AbstractSingleNoteInteractiveObject

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            Activate();
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            Deactivate();
        }
        
        #endregion
        #region Private utility

        void SetupLight()
        {
            m_light = gameObject.AddComponent<Light>();
            m_light.color = Music.NoteColours.GetColour(noteColour);
        }

        void Activate()
        {
            m_light.intensity = activateIntensity;
            
            m_rigidbody.AddForce(hoverDir * hoverStrength, ForceMode.Impulse);

            m_rigidbody.useGravity = false;

            m_isActive = true;
        }

        void Deactivate()
        {
            m_light.intensity = 0.0f;
            
            m_rigidbody.useGravity = true;
            
            m_rigidbody.AddForce(- hoverDir * hoverStrength, ForceMode.Impulse);

            m_isActive = false;
        }

        #endregion
        #region Private data

        Rigidbody m_rigidbody;
        Light m_light;

        bool m_isActive;

        #endregion
    }
}
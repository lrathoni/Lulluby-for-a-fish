
using UnityEngine;
using UnityEngine.Assertions;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that gets attracted to a position relative to the player when activated.
    /// Uses physics to animate. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class AbstractMagnetisedInteractiveObject : AbstractSingleNoteInteractiveObject
    {
        #region Serialised data
        
        public Vector3 relativePos = new Vector3(0f, 0f, 1f);
        [Range(0.01f, 100f)] public float strength = 20f;
        [Range(0.01f, 50f)] public float objectDrag = 2f;

        #endregion
        
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();
            
            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);
        }

        #endregion

        #region To resolve

        protected abstract Vector3 GetMagnetVector();

        #endregion
        
        #region AbstractSingleNoteInteractiveObject resolution

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            m_previousUseGravity = m_rigidbody.useGravity;
            m_previousDrag = m_rigidbody.drag;
            
            m_rigidbody.useGravity = false;
            m_rigidbody.drag = objectDrag;
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            m_rigidbody.useGravity = m_previousUseGravity;
            m_rigidbody.drag = m_previousDrag;
        }

        protected override void FixedUpdateActive()
        {
            ApplyMagnet(GetMagnetVector());
        }

        #endregion
        
        #region Private utility

        void ApplyMagnet(Vector3 magnetVec)
        {
            float r = magnetVec.magnitude;
            float amp = strength;
            // smoothing things over, so that the magnet's position is a resting position.
            amp *= r >= 1f
                ? 1f / (r * r)
                : r;
            m_rigidbody.AddForce(magnetVec.normalized * (Time.smoothDeltaTime * amp), ForceMode.Impulse);
        }

        #endregion
        
        #region Private data

        Rigidbody m_rigidbody;

        bool m_previousUseGravity;
        float m_previousDrag;

        #endregion
    }
}
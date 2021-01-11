
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

        public Vector3 hoverDir = Vector3.up;
        [Range(0.1f, 10.0f)]
        public float   hoverStrength = 3.0f;

        #endregion
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();

            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);

            hoverDir = hoverDir.normalized;
            
            Deactivate(null);
        }

        #endregion
        #region AbstractSingleNoteInteractiveObject

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            m_rigidbody.AddForce(hoverDir * hoverStrength, ForceMode.Impulse);

            m_rigidbody.useGravity = false;
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            m_rigidbody.useGravity = true;
            
            m_rigidbody.AddForce(- hoverDir * hoverStrength, ForceMode.Impulse);
        }
        
        #endregion
        #region Private utility

        #endregion
        #region Private data

        Rigidbody m_rigidbody;

        #endregion
    }
}
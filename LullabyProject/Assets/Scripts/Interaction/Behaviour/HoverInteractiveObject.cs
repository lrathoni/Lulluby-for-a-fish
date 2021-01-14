
using UnityEngine;
using UnityEngine.Assertions;

using MptUnity.Audio;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that hovers when activated.
    /// Uses physics to animate. 
    /// </summary>
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
            
        }

        #endregion
        
        #region AbstractSingleNoteInteractiveObject resolution

        protected override void Activate(MusicalNote note)
        {
            print($"Started: {note.tone}");
            m_rigidbody.useGravity = false;
            m_rigidbody.AddForce(hoverDir * hoverStrength, ForceMode.Force);
        }

        protected override void Deactivate(MusicalNote note)
        {
            print($"Stopped: {note.tone}"); 
            m_rigidbody.useGravity = true;
            m_rigidbody.AddForce(hoverDir * - hoverStrength, ForceMode.Force);
        }
        
        #endregion
        
        #region Private data

        Rigidbody m_rigidbody;

        #endregion
    }
}
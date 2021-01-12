
using UnityEngine;
using UnityEngine.Assertions;

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
            
            Deactivate(null);
        }

        #endregion
        #region AbstractSingleNoteInteractiveObject resolution

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            ++m_ayyy; 
            print($"Ayyyy: {m_ayyy}");
            m_rigidbody.AddForce(hoverDir * hoverStrength, ForceMode.Force);
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            --m_ayyy;
            print($"Nayyyy: {m_ayyy}");
            m_rigidbody.AddForce(hoverDir * - hoverStrength, ForceMode.Force);
        }
        
        #endregion
        #region Private utility

        #endregion
        #region Private data

        int m_ayyy;
        Rigidbody m_rigidbody;

        #endregion
    }
}
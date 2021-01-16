
using UnityEngine;
using UnityEngine.Assertions;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that gets attracted to a position relative to the player when activated.
    /// Uses physics to animate. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MagnetInteractiveObject : AbstractSingleNoteInteractiveObject
    {
        #region Serialised data
        
        public Vector3 magnetRelativePos = new Vector3(0f, 0f, 1f);
        [Range(0.01f, 100f)] public float magnetStrength = 20f;
        [Range(0.01f, 100f)] public float magnetDrag = 20f;

        #endregion
        
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();
            
            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);
        }

        #endregion
        
        #region AbstractSingleNoteInteractiveObject resolution

        protected override void Activate(MptUnity.Audio.MusicalNote note)
        {
            print("Start.");
            m_previousUseGravity = m_rigidbody.useGravity;
            m_previousDrag = m_rigidbody.drag;
            
            m_rigidbody.useGravity = false;
            m_rigidbody.drag = magnetDrag;
        }

        protected override void Deactivate(MptUnity.Audio.MusicalNote note)
        {
            print("Stop!!!");
            m_rigidbody.useGravity = m_previousUseGravity;
            m_rigidbody.drag = m_previousDrag;
        }

        protected override void FixedUpdateActive()
        {
            // Vector from magnet to object.
            Transform playerTrans = flutePlayer.GetFluteTransform();

            Vector3 localMagnetPos = playerTrans.localPosition + magnetRelativePos;
            Vector3 vec = playerTrans.TransformPoint(localMagnetPos) - transform.position;

            float r = vec.magnitude;
            float amp = magnetStrength;
            // smoothing things over, so that the magnet's position is a resting position.
            amp *= r >= 1f
                ? 1f / (r * r)
                : r;
            m_rigidbody.AddForce(vec.normalized * (Time.smoothDeltaTime * amp), ForceMode.Impulse);
            
        }

        #endregion
        
        #region Private utility

        #endregion
        
        #region Private data

        Rigidbody m_rigidbody;

        bool m_previousUseGravity;
        float m_previousDrag;

        #endregion
    }
}
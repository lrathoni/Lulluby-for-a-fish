
using UnityEngine;
using UnityEngine.Assertions;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that gets attracted to a position relative to the player when activated.
    /// Uses physics to animate. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(DistanceRangeModifier))]
    public class MagnetInteractiveObject : AbstractSingleNoteInteractiveObject
    {
        #region Serialised data
        
        public Vector3 magnetRelativePos = new Vector3(0f, 0f, 1f);
        [Range(0.01f, 5.0f)]
        public float magnetStrength = 1.0f;

        #endregion
        #region Unity Monobehaviour events
        
        protected override void Start()
        {
            base.Start();
            
            GameObject player = GameObject.FindWithTag("Player");
            Assert.IsNotNull(player);
            m_playerTrans = player.transform;

            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);

            m_distanceRangeModifier = GetComponent<DistanceRangeModifier>();
            Assert.IsNotNull(m_distanceRangeModifier);

            Deactivate(null);
        }

        #endregion
        #region AbstractSingleNoteInteractiveObject resolution

        protected override void FixedUpdateActive()
        {
            // Vector from magnet to object.
            Vector4 temp = m_playerTrans.localToWorldMatrix * (m_playerTrans.localPosition + magnetRelativePos);
            Vector3 vec = new Vector3(temp.x, temp.y, temp.z);
            vec = transform.position - vec;
            float strength = Mathf.Lerp(
                0f, 
                magnetStrength,
                Mathf.InverseLerp(m_distanceRangeModifier.maxDistance, 0f, vec.magnitude)
            ); 
            Debug.Log(vec);
            m_rigidbody.AddForce(vec * (strength * Time.smoothDeltaTime), ForceMode.Force);
        }

        #endregion
        #region Private utility

        #endregion
        #region Private data

        Transform m_playerTrans;
        Rigidbody m_rigidbody;
        DistanceRangeModifier m_distanceRangeModifier;

        #endregion
    }
}
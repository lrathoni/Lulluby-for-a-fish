
using UnityEngine;

namespace Interaction.Behaviour
{
    /// <summary>
    /// An interactive object that hovers when activated.
    /// Uses physics to animate. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HoverInteractiveObject : AbstractMagnetisedInteractiveObject
    {
        #region AbstractMagnetisedInteractiveObject

        protected override void Start()
        {
            base.Start();

            m_startingHeight = GetCurrentHeight();
        }

        protected override Vector3 GetMagnetVector()
        {
            return relativePos * (GetTargetHeight() - GetCurrentHeight());
        }

        #endregion

        #region Private utility

        float GetCurrentHeight()
        {
            return Vector3.Dot(transform.position, relativePos.normalized);
        }

        float GetTargetHeight()
        {
            return relativePos.magnitude;
        }

        #endregion
        
        #region Private data

        float m_startingHeight;

        #endregion
    }
}
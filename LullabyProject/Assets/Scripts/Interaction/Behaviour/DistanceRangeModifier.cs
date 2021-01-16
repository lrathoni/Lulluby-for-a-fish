
using UnityEngine;

using MptUnity.Audio;
using IO.Behaviour;

namespace Interaction.Behaviour
{
    public class DistanceRangeModifier : AbstractInteractiveRangeModifier
    {
        #region Serialised data

        [Range(0.0f, 10f)]
        public float maxDistance;
        
        #endregion
        #region AbstractInteractiveRangeModifier resolution

        public override bool IsInRange(FlutePlayer flutePlayer)
        {
            return Vector3.Distance(flutePlayer.transform.position, transform.position) <= maxDistance;
        }

        #endregion
    }
}
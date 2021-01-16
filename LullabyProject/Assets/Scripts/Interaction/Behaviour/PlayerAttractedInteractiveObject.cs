
using UnityEngine;

namespace Interaction.Behaviour
{
    public class PlayerAttractedInteractiveObject : AbstractMagnetisedInteractiveObject
    {
        #region AbstractMagnetisedInteractiveObject resolution

        protected override Vector3 GetMagnetVector()
        {
            // Vector from magnet to object.
            Transform playerTrans = flutePlayer.GetFluteTransform();

            Vector3 localMagnetPos = playerTrans.localPosition + relativePos;
            return playerTrans.TransformPoint(localMagnetPos) - transform.position;
        }

        #endregion
    }
}
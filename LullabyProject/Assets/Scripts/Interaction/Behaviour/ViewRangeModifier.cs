
using UnityEngine;
using UnityEngine.Assertions;
using IO.Behaviour;

namespace Interaction.Behaviour
{
    public class ViewRangeModifier : AbstractInteractiveRangeModifier
    {
        #region Serialised data

        [Range(0.0f, 10f)]
        public float maxDistance;
        [Range(0.0f, 0.5f)] public float maxOffset;
        
        #endregion
        #region AbstractInteractiveRangeModifier resolution

        public override bool IsInRange(FlutePlayer flutePlayer)
        {
            Camera cam = Camera.main;
            // is not null, assert!
            Assert.IsNotNull(cam);
            Transform camTrans = cam.transform;

            Vector3 vec = gameObject.transform.position - camTrans.position;
            
            // first check that it is in bounds
            // The offset is the projection of the vector on the camera surface
            // normalise it wrt. screen dimension
            Vector3 proj = cam.WorldToViewportPoint(gameObject.transform.position);
            Vector2 screenVecProj = new Vector2(proj.x - 0.5f, proj.y - 0.5f);
            Debug.Log(screenVecProj);
            if (proj.z < 0 || screenVecProj.magnitude > maxOffset)
            {
                // Not a candidate. Return early, 
                return false;
            }

            RaycastHit hit;
            bool hasHit = Physics.Raycast(
                camTrans.position,
                vec.normalized,
                out hit,
                maxDistance,
                0
                );
            // If the ray did not hit anything, or if the collider is this object's own.
            // This is done so that we don't require the interactive object to have a collider.
            return !hasHit || hit.collider.gameObject == gameObject;
        }

        #endregion
    }
}
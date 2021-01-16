using IO.Behaviour;

using MptUnity.Audio;

namespace Interaction.Behaviour
{
    /// <summary>
    /// Strategies for filtering interactive objects when playing according to range.
    /// </summary>
    public abstract class AbstractInteractiveRangeModifier : UnityEngine.MonoBehaviour
    {
        #region To resolve

        public abstract bool IsInRange(FlutePlayer flutePlayer);

        #endregion
    }
}
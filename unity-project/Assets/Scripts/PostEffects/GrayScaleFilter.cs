using UnityEngine;
using UnityEngine.Rendering;

namespace Meren.PostEffects
{
    [System.Serializable]
    [VolumeComponentMenu("meren/GrayScale Filter")]
    public class GrayScaleFilter : VolumeComponent
    {
        public bool IsActive() => m_threshold.overrideState && m_threshold.value > 0;
        public ClampedFloatParameter m_threshold = new ClampedFloatParameter(0f, 0f, 1f);
    }
}

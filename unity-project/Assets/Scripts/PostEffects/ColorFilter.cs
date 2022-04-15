using UnityEngine;
using UnityEngine.Rendering;

namespace Meren.PostEffects
{
    [System.Serializable]
    [VolumeComponentMenu("meren/Color Filter")]
    public class ColorFilter : VolumeComponent
    {
        public bool IsActive() => m_filterColor.overrideState && m_filterColor.value.a > 0;
        public ColorParameter m_filterColor = new ColorParameter(Color.white);
    }
}

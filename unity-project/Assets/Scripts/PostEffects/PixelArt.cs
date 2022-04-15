using UnityEngine;
using UnityEngine.Rendering;

namespace Meren.PostEffects
{
    [System.Serializable]
    [VolumeComponentMenu("meren/Pixel Art")]
    public class PixelArt : VolumeComponent
    {
        public bool IsActive() => m_roughness.overrideState && active;
        public Vector3Parameter m_roughness = new Vector3Parameter(new Vector3(64, 64, 64));
    }
}

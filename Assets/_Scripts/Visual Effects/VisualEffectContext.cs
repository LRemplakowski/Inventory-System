using UnityEngine;
using UnityEngine.Rendering;

namespace SunsetSystems.VisualEffects
{
    public struct VisualEffectContext
    {
        public float Duration;
        public Transform FollowTransform;
        public Transform PlayerCameraRig;
        public Vector3 Position;
        public VolumeProfile VolumeProfile;
        public bool SelfGovernDuration;
        public MonoBehaviour CoroutineRunner;
    }
}

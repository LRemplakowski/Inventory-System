using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public interface IVisualEffect
    {
        string ID { get; }
        GameObject GameObject { get; }
        VisualEffectParent ParentingType { get; }

        void SelfGovernDuration();
        void SetFollowTransform(Transform follow);
    }

    public enum VisualEffectParent
    {
        None, Handler
    }
}

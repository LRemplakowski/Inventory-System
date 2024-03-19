using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public interface IVisualEffect
    {
        string InstanceID { get; }
        bool AllowOnlySingleInstance { get; }
        GameObject GameObject { get; }
        VisualEffectParent ParentingType { get; }

        void Initialize(VisualEffectContext context);

        void AddDuration(float duration);
    }

    public enum VisualEffectParent
    {
        None, Handler
    }
}

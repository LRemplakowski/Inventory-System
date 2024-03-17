using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public interface IOrbital
    {
        string ID { get; }
        Transform OrbitalTransform { get; }
        float RotationOffset { get; }
        float RotationSpeed { get; }
    }
}

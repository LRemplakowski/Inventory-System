using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public static class ColorExtentions
    {
        public static float InverseLerp(Color a, Color b, Color color)
        {
            float _lerpRed = Mathf.InverseLerp(a.r, b.r, color.r);
            float _lerpGreen = Mathf.InverseLerp(a.g, b.g, color.g);
            float _lerpBlue = Mathf.InverseLerp(a.b, b.b, color.b);
            float _lerpAlpha = Mathf.InverseLerp(a.a, b.a, color.a);

            float _lerp = Mathf.Max(_lerpRed, Mathf.Max(_lerpGreen, Mathf.Max(_lerpBlue, _lerpAlpha)));
            return _lerp;
        }
    }
}

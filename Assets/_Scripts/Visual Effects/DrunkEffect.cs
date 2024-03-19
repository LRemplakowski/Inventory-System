using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SunsetSystems.VisualEffects
{
    public class DrunkEffect : SerializedMonoBehaviour, IVisualEffect
    {
        [SerializeField]
        private float _defaultRestoreTime = 1f;
        [SerializeField]
        private LensDistortionSettings _lensDistortionSettings;
        [SerializeField]
        private PaniniProjectionSettings _paniniProjectionSettings;
        [SerializeField]
        private VignetteSettings _vignetteSettings;
        [SerializeField]
        private CameraRigSettings _cameraRigSettings;

        public string InstanceID => GetInstanceID().ToString();

        public GameObject GameObject => gameObject;

        public VisualEffectParent ParentingType => VisualEffectParent.None;

        public bool AllowOnlySingleInstance => true;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private float _effectDuration;
        private CachedEffectValues _defaultValuesCache = null;
        [ShowInInspector, ReadOnly]
        private VisualEffectContext _cachedEffectContext;

        private IEnumerator _drunkCoroutine;

        private LensDistortion _lensDistortion;
        private PaniniProjection _paniniProjection;
        private Vignette _vignette;

        private void OnDestroy()
        {
            if (_cachedEffectContext.CoroutineRunner != null)
                _cachedEffectContext.CoroutineRunner.StopCoroutine(_drunkCoroutine);
            RestoreDefaultSettings(_defaultValuesCache);
        }

        public void Initialize(VisualEffectContext context)
        {
            _cachedEffectContext = context;
            context.VolumeProfile.TryGet(out _lensDistortion);
            context.VolumeProfile.TryGet(out _paniniProjection);
            context.VolumeProfile.TryGet(out _vignette);
            CacheDefaultSettings(_lensDistortion, _paniniProjection, _vignette, context.PlayerCameraRig);
            _drunkCoroutine = DrunkEffectLifetime();
            context.CoroutineRunner.StartCoroutine(_drunkCoroutine);
        }

        private IEnumerator DrunkEffectLifetime()
        {
            _effectDuration = _cachedEffectContext.Duration;
            _lensDistortionSettings.Initialize(_lensDistortion);
            while (_effectDuration > _defaultRestoreTime)
            {
                float timeDelta = Time.deltaTime;
                if (_lensDistortion != null)
                    _lensDistortionSettings.UpdateLensDistortion(timeDelta, _lensDistortion);
                _effectDuration -= timeDelta;
                yield return null;
            }
            _cachedEffectContext.CoroutineRunner.StartCoroutine(RestoreDefaultValuesOverTime(_defaultRestoreTime, _defaultValuesCache));
        }

        private IEnumerator RestoreDefaultValuesOverTime(float time, CachedEffectValues defaultValues)
        {
            yield break;
        }    

        private void CacheDefaultSettings(LensDistortion lensDistortion, PaniniProjection paniniProjection, Vignette vignette, Transform cameraRigTransform)
        {
            _defaultValuesCache = new();
            LensDistortion cachedDistortion = new();
            cachedDistortion.intensity.value = lensDistortion.intensity.value;
            cachedDistortion.xMultiplier.value = lensDistortion.xMultiplier.value;
            cachedDistortion.yMultiplier.value = lensDistortion.yMultiplier.value;
            cachedDistortion.center.value = lensDistortion.center.value;
            cachedDistortion.scale.value = lensDistortion.scale.value;
            _defaultValuesCache.LensDistortion = cachedDistortion;
        }

        private void RestoreDefaultSettings(CachedEffectValues defaultValues)
        {
            LensDistortion cachedLensDistortion = defaultValues.LensDistortion;
            _lensDistortion.intensity.value = cachedLensDistortion.intensity.value;
            _lensDistortion.xMultiplier.value = cachedLensDistortion.xMultiplier.value;
            _lensDistortion.yMultiplier.value = cachedLensDistortion.yMultiplier.value;
            _lensDistortion.center.value = cachedLensDistortion.center.value;
            _lensDistortion.scale.value = cachedLensDistortion.scale.value;
        }

        public void AddDuration(float duration)
        {
            _effectDuration += duration;
        }

        private class CachedEffectValues
        {
            public LensDistortion LensDistortion;
            public PaniniProjection PaniniProjection;
            public Vignette Vignette;
            public Vector3 CameraRigRotation;
        }

        [Serializable]
        private class LensDistortionSettings
        {
            [MinMaxSlider(-1f, 1f, true)]
            public Vector2 IntensityRange = new(-.5f, .5f);
            [Min(0)]
            public float IntensityLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 XMultiplierRange = new(.25f, .75f);
            [Min(0)]
            public float XMultiplierLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 YMultiplierRange = new(.25f, .75f);
            [Min(0)]
            public float YMultiplierLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 CenterXRange = new(.25f, .75f);
            [Min(0)]
            public float CenterXLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 CenterYRange = new(.25f, .75f);
            [Min(0)]
            public float CenterYLerpSpeed = 1f;
            [MinMaxSlider(0.01f, 5f, true)]
            public Vector2 ScaleRange = new(.75f, 1.25f);
            [Min(0)]
            public float ScaleLerpSpeed = 1f;

            private float _intensityLerp = 0f;
            private float _xMultiplierLerp = 0f;
            private float _yMultiplierLerp = 0f;
            private float _centerXLerp = 0f;
            private float _centerYLerp = 0f;
            private float _scaleLerp = 0f;

            public void Initialize(LensDistortion lensDistortion)
            {
                if (lensDistortion == null)
                {
                    Debug.LogError($"Lens Distortion effect was given a null LensDistortion reference!");
                    return;
                }
                _intensityLerp = Mathf.InverseLerp(IntensityRange.x, IntensityRange.y, lensDistortion.intensity.value);
                _xMultiplierLerp = Mathf.InverseLerp(XMultiplierRange.x, XMultiplierRange.y, lensDistortion.xMultiplier.value);
                _yMultiplierLerp = Mathf.InverseLerp(YMultiplierRange.x, YMultiplierRange.y, lensDistortion.yMultiplier.value);
                _centerXLerp = Mathf.InverseLerp(CenterXRange.x, CenterXRange.y, lensDistortion.center.value.x);
                _centerYLerp = Mathf.InverseLerp(CenterYRange.x, CenterYRange.y, lensDistortion.center.value.y);
                _scaleLerp = Mathf.InverseLerp(ScaleRange.x, ScaleRange.y, lensDistortion.scale.value);
            }

            public void UpdateLensDistortion(float deltaTime, LensDistortion lensDistortion)
            {
                _intensityLerp += deltaTime * IntensityLerpSpeed;
                PingPongLerp(ref _intensityLerp, ref IntensityLerpSpeed);
                lensDistortion.intensity.value = Mathf.Lerp(IntensityRange.x, IntensityRange.y, _intensityLerp);

                _xMultiplierLerp += deltaTime * XMultiplierLerpSpeed;
                PingPongLerp(ref _xMultiplierLerp, ref XMultiplierLerpSpeed);
                lensDistortion.xMultiplier.value = Mathf.Lerp(XMultiplierRange.x, XMultiplierRange.y, _xMultiplierLerp);

                _yMultiplierLerp += deltaTime * YMultiplierLerpSpeed;
                PingPongLerp(ref _yMultiplierLerp, ref YMultiplierLerpSpeed);
                lensDistortion.yMultiplier.value = Mathf.Lerp(YMultiplierRange.x, YMultiplierRange.y, _yMultiplierLerp);

                Vector2 centerValue = new();
                _centerXLerp += deltaTime * CenterXLerpSpeed;
                PingPongLerp(ref _centerXLerp, ref CenterXLerpSpeed);
                centerValue.x = Mathf.Lerp(CenterXRange.x, CenterXRange.y, _centerXLerp);
                _centerYLerp += deltaTime * CenterYLerpSpeed;
                PingPongLerp(ref _centerYLerp, ref CenterYLerpSpeed);
                centerValue.y = Mathf.Lerp(CenterYRange.x, CenterYRange.y, _centerYLerp);
                lensDistortion.center.value = centerValue;

                _scaleLerp += deltaTime * ScaleLerpSpeed;
                PingPongLerp(ref _scaleLerp, ref ScaleLerpSpeed);
                lensDistortion.scale.value = Mathf.Lerp(ScaleRange.x, ScaleRange.y, _scaleLerp);

            }

            private void PingPongLerp(ref float lerpValue, ref float lerpSpeed)
            {
                if (lerpValue >= 1 || lerpValue <= 0)
                {
                    lerpValue = Mathf.Clamp01(lerpValue);
                    lerpSpeed = -lerpSpeed;
                }
            }
        }

        [Serializable]
        private class PaniniProjectionSettings
        {

        }

        [Serializable]
        private class VignetteSettings
        {

        }

        [Serializable]
        private class CameraRigSettings
        {

        }
    }
}

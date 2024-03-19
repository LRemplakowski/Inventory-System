using System;
using System.Collections;
using Sirenix.OdinInspector;
using SunsetSystems.Utils;
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
            _paniniProjectionSettings.Initialize(_paniniProjection);
            _vignetteSettings.Initialize(_vignette);
            _cameraRigSettings.Initialize(_cachedEffectContext.PlayerCameraRig);
            while (_effectDuration > _defaultRestoreTime)
            {
                float timeDelta = Time.deltaTime;
                if (_lensDistortion != null)
                    _lensDistortionSettings.UpdateLensDistortion(timeDelta, _lensDistortion);
                if (_paniniProjection != null)
                    _paniniProjectionSettings.UpdatePaniniProjection(timeDelta, _paniniProjection);
                if (_vignette != null)
                    _vignetteSettings.UpdateVignette(timeDelta, _vignette);
                if (_cachedEffectContext.PlayerCameraRig != null)
                    _cameraRigSettings.UpdateCameraRig(timeDelta, _cachedEffectContext.PlayerCameraRig);
                _effectDuration -= timeDelta;
                yield return null;
            }
            _cachedEffectContext.CoroutineRunner.StartCoroutine(RestoreDefaultValuesOverTime(_defaultRestoreTime, _defaultValuesCache));
        }

        private IEnumerator RestoreDefaultValuesOverTime(float duration, CachedEffectValues defaultValues)
        {
            if (duration <= 0f)
            {
                RestoreDefaultSettings(defaultValues);
                yield break;
            }
            float timeElapsed = 0f;
            float lensIntensity = _lensDistortion.intensity.value;
            float lensXMultiplier = _lensDistortion.xMultiplier.value;
            float lensYMultiplier = _lensDistortion.yMultiplier.value;
            Vector2 lensCenter = _lensDistortion.center.value;
            float lensScale = _lensDistortion.scale.value;
            float paniniDistance = _paniniProjection.distance.value;
            float paniniCrop = _paniniProjection.cropToFit.value;
            Color vignetteColor = _vignette.color.value;
            Vector2 vignetteCenter = _vignette.center.value;
            float vignetteIntensity = _vignette.intensity.value;
            float vignetteSmoothness = _vignette.smoothness.value;
            Quaternion cameraRotation = _cachedEffectContext.PlayerCameraRig.localRotation;
            LensDistortion cachedLensDistortion = defaultValues.LensDistortion;
            PaniniProjection cachedPaniniProjection = defaultValues.PaniniProjection;
            Vignette cachedVignette = defaultValues.Vignette;
            while (timeElapsed / duration < 1f)
            {
                timeElapsed += Time.deltaTime;
                float lerp = timeElapsed / duration;
                _lensDistortion.intensity.value = Mathf.Lerp(lensIntensity, cachedLensDistortion.intensity.value, lerp);
                _lensDistortion.xMultiplier.value = Mathf.Lerp(lensXMultiplier, cachedLensDistortion.xMultiplier.value, lerp);
                _lensDistortion.yMultiplier.value = Mathf.Lerp(lensYMultiplier, cachedLensDistortion.yMultiplier.value, lerp);
                _lensDistortion.center.value = Vector2.Lerp(lensCenter, cachedLensDistortion.center.value, lerp);
                _lensDistortion.scale.value = Mathf.Lerp(lensScale, cachedLensDistortion.scale.value, lerp);
                _paniniProjection.distance.value = Mathf.Lerp(paniniDistance, cachedPaniniProjection.distance.value, lerp);
                _paniniProjection.cropToFit.value = Mathf.Lerp(paniniCrop, cachedPaniniProjection.cropToFit.value, lerp);
                _vignette.color.value = Color.Lerp(vignetteColor, cachedVignette.color.value, lerp);
                _vignette.center.value = Vector2.Lerp(vignetteCenter, cachedVignette.center.value, lerp);
                _vignette.intensity.value = Mathf.Lerp(vignetteIntensity, cachedVignette.intensity.value, lerp);
                _vignette.smoothness.value = Mathf.Lerp(vignetteSmoothness, cachedVignette.smoothness.value, lerp);
                _cachedEffectContext.PlayerCameraRig.localRotation = Quaternion.Lerp(cameraRotation, defaultValues.CameraRigRotation, lerp);
                yield return null;
            }
            RestoreDefaultSettings(defaultValues);
        }    

        private void CacheDefaultSettings(LensDistortion lensDistortion, PaniniProjection paniniProjection, Vignette vignette, Transform cameraRigTransform)
        {
            _defaultValuesCache = new();
            LensDistortion cachedDistortion = ScriptableObject.CreateInstance<LensDistortion>();
            cachedDistortion.intensity.value = lensDistortion.intensity.value;
            cachedDistortion.xMultiplier.value = lensDistortion.xMultiplier.value;
            cachedDistortion.yMultiplier.value = lensDistortion.yMultiplier.value;
            cachedDistortion.center.value = lensDistortion.center.value;
            cachedDistortion.scale.value = lensDistortion.scale.value;
            _defaultValuesCache.LensDistortion = cachedDistortion;
            PaniniProjection cachedProjection = ScriptableObject.CreateInstance<PaniniProjection>();
            cachedProjection.distance.value = paniniProjection.distance.value;
            cachedProjection.cropToFit.value = paniniProjection.cropToFit.value;
            _defaultValuesCache.PaniniProjection = cachedProjection;
            Vignette cachedVignette = ScriptableObject.CreateInstance<Vignette>();
            cachedVignette.color.value = vignette.color.value;
            cachedVignette.center.value = vignette.center.value;
            cachedVignette.intensity.value = vignette.intensity.value;
            cachedVignette.smoothness.value = vignette.smoothness.value;
            _defaultValuesCache.Vignette = cachedVignette;
            _defaultValuesCache.CameraRigRotation = cameraRigTransform.localRotation;
        }

        private void RestoreDefaultSettings(CachedEffectValues defaultValues)
        {
            if (defaultValues == null)
                return;
            LensDistortion cachedLensDistortion = defaultValues.LensDistortion;
            _lensDistortion.intensity.value = cachedLensDistortion.intensity.value;
            _lensDistortion.xMultiplier.value = cachedLensDistortion.xMultiplier.value;
            _lensDistortion.yMultiplier.value = cachedLensDistortion.yMultiplier.value;
            _lensDistortion.center.value = cachedLensDistortion.center.value;
            _lensDistortion.scale.value = cachedLensDistortion.scale.value;
            PaniniProjection cachedPaniniProjection = defaultValues.PaniniProjection;
            _paniniProjection.distance.value = cachedPaniniProjection.distance.value;
            _paniniProjection.cropToFit.value = cachedPaniniProjection.cropToFit.value;
            Vignette cachedVignette = defaultValues.Vignette;
            _vignette.color.value = cachedVignette.color.value;
            _vignette.center.value = cachedVignette.center.value;
            _vignette.intensity.value = cachedVignette.intensity.value;
            _vignette.smoothness.value = cachedVignette.smoothness.value;
            if (_cachedEffectContext.PlayerCameraRig != null)
                _cachedEffectContext.PlayerCameraRig.localRotation = defaultValues.CameraRigRotation;
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
            public Quaternion CameraRigRotation;
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
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 DistanceRange = new(0, 1);
            [Min(0f)]
            public float DistanceLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 CropToFitRange = new(0, 1);
            [Min(0f)]
            public float CropToFitLerpSpeed = 1f;

            private float _distanceLerp;
            private float _cropToFitLerp;

            public void Initialize(PaniniProjection panini)
            {
                if (panini == null)
                {
                    Debug.LogError($"Panini Projection effect was given a null PaniniProjection reference!");
                    return;
                }
                _distanceLerp = Mathf.InverseLerp(DistanceRange.x, DistanceRange.y, panini.distance.value);
                _cropToFitLerp = Mathf.InverseLerp(CropToFitRange.x, CropToFitRange.y, panini.cropToFit.value);
            }

            public void UpdatePaniniProjection(float timeDelta, PaniniProjection panini)
            {
                _distanceLerp += timeDelta * DistanceLerpSpeed;
                PingPongLerp(ref _distanceLerp, ref DistanceLerpSpeed);
                panini.distance.value = Mathf.Lerp(DistanceRange.x, DistanceRange.y, _distanceLerp);

                _cropToFitLerp += timeDelta * CropToFitLerpSpeed;
                PingPongLerp(ref _cropToFitLerp, ref DistanceLerpSpeed);
                panini.cropToFit.value = Mathf.Lerp(CropToFitRange.x, CropToFitRange.y, _cropToFitLerp);
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
        private class VignetteSettings
        {
            public Gradient ColorRange = new();
            [Min(0)]
            public float ColorLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 CenterXRange = new(.25f, .75f);
            [Min(0)]
            public float CenterXLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 CenterYRange = new(.25f, .75f);
            [Min(0)]
            public float CenterYLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 IntensityRange = new(0f, .5f);
            [Min(0)]
            public float IntensityLerpSpeed = 1f;
            [MinMaxSlider(0f, 1f, true)]
            public Vector2 SmoothnessRange = new(.2f, .5f);
            [Min(0f)]
            public float SmoothnessLerpSpeed = 1f;


            private float _colorLerp;
            private float _centerXLerp;
            private float _centerYLerp;
            private float _intensityLerp;
            private float _smoothnessLerp;

            public void Initialize(Vignette vignette)
            {
                _colorLerp = ColorExtentions.InverseLerp(ColorRange.Evaluate(0f), ColorRange.Evaluate(1f), vignette.color.value);
                _centerXLerp = Mathf.InverseLerp(CenterXRange.x, CenterXRange.y, vignette.center.value.x);
                _centerYLerp = Mathf.InverseLerp(CenterYRange.x, CenterYRange.y, vignette.center.value.y);
                _intensityLerp = Mathf.InverseLerp(IntensityRange.x, IntensityRange.y, vignette.intensity.value);
                _smoothnessLerp = Mathf.InverseLerp(SmoothnessRange.x, SmoothnessRange.y, vignette.smoothness.value);
            }

            public void UpdateVignette(float timeDelta, Vignette vignette)
            {
                _colorLerp += timeDelta * ColorLerpSpeed;
                PingPongLerp(ref _colorLerp, ref ColorLerpSpeed);
                vignette.color.value = ColorRange.Evaluate(_colorLerp);

                Vector2 centerValue = new();
                _centerXLerp += timeDelta * CenterXLerpSpeed;
                PingPongLerp(ref _centerXLerp, ref CenterXLerpSpeed);
                centerValue.x = Mathf.Lerp(CenterXRange.x, CenterXRange.y, _centerXLerp);
                _centerYLerp += timeDelta * CenterYLerpSpeed;
                PingPongLerp(ref _centerYLerp, ref CenterYLerpSpeed);
                centerValue.y = Mathf.Lerp(CenterYRange.x, CenterYRange.y, _centerYLerp);
                vignette.center.value = centerValue;

                _intensityLerp += timeDelta * IntensityLerpSpeed;
                PingPongLerp(ref _intensityLerp, ref IntensityLerpSpeed);
                vignette.intensity.value = Mathf.Lerp(IntensityRange.x, IntensityRange.y, _intensityLerp);

                _smoothnessLerp = timeDelta * SmoothnessLerpSpeed;
                PingPongLerp(ref _smoothnessLerp, ref SmoothnessLerpSpeed);
                vignette.smoothness.value = Mathf.Lerp(SmoothnessRange.x, SmoothnessRange.y, _smoothnessLerp);
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
        private class CameraRigSettings
        {
            [MinMaxSlider(-360f, 360f, true)]
            public Vector2 XRotationRange = new(-5f, 5f);
            [Min(0)]
            public float XRotationSpeed = 1f;
            [MinMaxSlider(-360f, 360f, true)]
            public Vector2 YRotationRange = new(-5f, 5f);
            [Min(0)]
            public float YRotationSpeed = 1f;
            [MinMaxSlider(-360f, 360f, true)]
            public Vector2 ZRotationRange = new(-10f, 10f);
            [Min(0)]
            public float ZRotationSpeed = 1f;

            private float _xLerp;
            private float _yLerp;
            private float _zLerp;

            public void Initialize(Transform rigTransform)
            {
                _xLerp = Mathf.InverseLerp(XRotationRange.x, XRotationRange.y, rigTransform.localRotation.eulerAngles.x);
                _yLerp = Mathf.InverseLerp(YRotationRange.x, YRotationRange.y, rigTransform.localRotation.eulerAngles.y);
                _zLerp = Mathf.InverseLerp(ZRotationRange.x, ZRotationRange.y, rigTransform.localRotation.eulerAngles.z);
            }

            public void UpdateCameraRig(float timeDelta, Transform rigTransform)
            {
                Vector3 cameraRotation = new();
                _xLerp += timeDelta * XRotationSpeed;
                PingPongLerp(ref _xLerp, ref XRotationSpeed);
                cameraRotation.x = Mathf.Lerp(XRotationRange.x, XRotationRange.y, _xLerp);
                _yLerp += timeDelta * YRotationSpeed;
                PingPongLerp(ref _yLerp, ref YRotationSpeed);
                cameraRotation.y = Mathf.Lerp(YRotationRange.x, YRotationRange.y, _yLerp);
                _zLerp += timeDelta * ZRotationSpeed;
                PingPongLerp(ref _zLerp, ref ZRotationSpeed);
                cameraRotation.z = Mathf.Lerp(ZRotationRange.x, ZRotationRange.y, _zLerp);
                rigTransform.localRotation = Quaternion.Euler(cameraRotation);
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
    }
}

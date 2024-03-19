using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public class FluidSplashEffect : SerializedMonoBehaviour, IVisualEffect
    {
        [SerializeField]
        private float _poolWarmupTime = .5f;
        [SerializeField]
        private Renderer _splashRenderer;
        [SerializeField]
        private List<MaterialPropertyData> _lerpProperties;

        public string InstanceID => GetInstanceID().ToString();
        public GameObject GameObject => gameObject;
        public VisualEffectParent ParentingType => VisualEffectParent.None;

        public bool AllowOnlySingleInstance => false;

        private IEnumerator _splashLifetimeCoroutine;

        [Title("Runtime")]
        [ShowInInspector]
        private VisualEffectContext _cachedContext;

        private float _selfDestructTime;

        public void SelfGovernDuration()
        {
            _selfDestructTime = _cachedContext.Duration;
            _cachedContext.CoroutineRunner.StartCoroutine(SelfDestructAfterTime());
        }

        private IEnumerator SelfDestructAfterTime()
        {
            while (_selfDestructTime > 0)
            {
                _selfDestructTime -= Time.deltaTime;
                yield return null;
            }
            if (gameObject != null)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_cachedContext.CoroutineRunner != null && _splashLifetimeCoroutine != null)
                _cachedContext.CoroutineRunner.StopCoroutine(_splashLifetimeCoroutine);
        }

        public void Initialize(VisualEffectContext context)
        {
            _cachedContext = context;
            transform.SetParent(context.FollowTransform);
            transform.position = context.Position;
            _splashLifetimeCoroutine = SplashLifetime();
            context.CoroutineRunner.StartCoroutine(_splashLifetimeCoroutine);
        }

        private IEnumerator SplashLifetime()
        {
            if (_cachedContext.Duration <= 0 || _poolWarmupTime > _cachedContext.Duration)
                yield break;
            float time = 0f;
            float lerpDuration = _poolWarmupTime;
            while (time < lerpDuration)
            {
                foreach (MaterialPropertyData materialProperty in _lerpProperties)
                {
                    _splashRenderer.material.SetFloat(materialProperty.PropertyName, Mathf.Lerp(materialProperty.StartValue, materialProperty.EndValue, time / lerpDuration));
                }
                time += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(_cachedContext.Duration - (_poolWarmupTime * 2));
            while (_selfDestructTime > _poolWarmupTime)
            {
                yield return null;
            }
            time = 0f;
            while (time < lerpDuration)
            {
                foreach (MaterialPropertyData materialProperty in _lerpProperties)
                {
                    _splashRenderer.material.SetFloat(materialProperty.PropertyName, Mathf.Lerp(materialProperty.EndValue, materialProperty.StartValue, time / lerpDuration));
                }
                time += Time.deltaTime;
                yield return null;
            }
            _splashLifetimeCoroutine = null;
        }

        public void AddDuration(float duration)
        {
            _selfDestructTime += duration;
        }

        [Serializable]
        private struct MaterialPropertyData
        {
            public string PropertyName;
            public float StartValue, EndValue;
        }
    }
}

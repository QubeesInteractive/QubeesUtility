using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class QUIWindow : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        public List<QWindowEffect> OpeningEffectList = new();
        public List<QWindowEffect> ClosingEffectList = new();

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        [Button]
        public virtual void Open()
        {
            gameObject.SetActive(true);
            QUIManager.OpenQWindow?.Invoke(this);
            StartCoroutine(PlayEffects(OpeningEffectList));
        }

        [Button]
        public virtual void Close()
        {
            StartCoroutine(PlayEffects(ClosingEffectList, onComplete: () => gameObject.SetActive(false)));
        }

        private IEnumerator PlayEffects(List<QWindowEffect> effects, Action onComplete = null)
        {
            var activeCoroutines = new List<Coroutine>();
            foreach (var effect in effects)
            {
                switch (effect.type)
                {
                    case QUIWindowEffectType.FadeIn:
                        _canvasGroup.alpha = 0; // Start value
                        activeCoroutines.Add(StartCoroutine(FadeIn(effect.effectDuration)));
                        break;
                    case QUIWindowEffectType.FadeOut:
                        _canvasGroup.alpha = 1; // Start value
                        activeCoroutines.Add(StartCoroutine(FadeOut(effect.effectDuration)));
                        break;
                    case QUIWindowEffectType.ScaleUp:
                        transform.localScale = Vector3.zero; // Start value
                        activeCoroutines.Add(StartCoroutine(ScaleUp(effect.effectDuration)));
                        break;
                    case QUIWindowEffectType.ScaleDown:
                        transform.localScale = Vector3.one; // Start value
                        activeCoroutines.Add(StartCoroutine(ScaleDown(effect.effectDuration)));
                        break;
                }
            }
            
            foreach (var coroutine in activeCoroutines)
            {
                yield return coroutine;
            }

            onComplete?.Invoke();
        }

        private IEnumerator FadeIn(float duration = 0.5f)
        {
            var startAlpha = _canvasGroup.alpha;
            var time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, Mathf.SmoothStep(0, 1, time / duration));
                yield return null;
            }

            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private IEnumerator FadeOut(float duration = 0.5f)
        {
            var startAlpha = _canvasGroup.alpha;
            var time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, Mathf.SmoothStep(0, 1, time / duration));
                yield return null;
            }

            _canvasGroup.alpha = 0;
        }

        private IEnumerator ScaleUp(float duration = 0.5f)
        {
            var startScale = transform.localScale;
            var targetScale = Vector3.one;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0, 1, time / duration));
                yield return null;
            }

            transform.localScale = targetScale;
        }

        private IEnumerator ScaleDown(float duration = 0.5f)
        {
            var startScale = transform.localScale;
            var targetScale = Vector3.zero;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0, 1, time / duration));
                yield return null;
            }

            transform.localScale = targetScale;
        }
    }

    [Serializable]
    public class QWindowEffect : ISerializationCallbackReceiver
    {
        public string effectName;
        public QUIWindowEffectType type;
        public float effectDuration;

        public void OnBeforeSerialize()
        {
            effectName = type.ToString();
        }

        public void OnAfterDeserialize()
        {
        }
    }

    public enum QUIWindowEffectType
    {
        FadeIn,
        FadeOut,
        ScaleUp,
        ScaleDown
    }
}
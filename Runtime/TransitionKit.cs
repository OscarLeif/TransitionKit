using System;
using System.Collections;
using UnityEngine;

namespace AtaGames.TransitionKit
{
    public class TransitionKit : MonoBehaviour
    {
        public static TransitionKit Get;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            GameObject gameObject = new GameObject(nameof(TransitionKit));
            Get = gameObject.AddComponent<TransitionKit>();
            DontDestroyOnLoad(gameObject);
        }

        public void InvokeOnTransitionStart() => OnTransitionStart?.Invoke();
        public void InvokeOnTransitionEnd() => OnTransitionEnd?.Invoke();
        public void InvokeBeforeSceneLoad() => BeforeSceneLoad?.Invoke();
        public void InvokeAfterSceneLoad() => AfterSceneLoad?.Invoke();

        public bool IsWorking => isWorking;

        public FadeTransition fadeTransition;
        public OpenCircleTransition openCircleTransition;

        public bool isWorking = false;

        public string NextSceneName;
        public int NextSceneIndex;

        public event Action OnTransitionStart;
        public event Action OnTransitionEnd;
        public event Action BeforeSceneLoad;
        public event Action AfterSceneLoad;

        public bool Initialize;

        private void Awake()
        {
            GameObject fadeGO = new GameObject(nameof(FadeTransition));
            fadeGO.transform.parent = transform;
            fadeTransition = fadeGO.AddComponent<FadeTransition>();
            fadeTransition.TransitionKit = this;

            GameObject circleGO = new GameObject(nameof(OpenCircleTransition));
            circleGO.transform.parent = transform;
            openCircleTransition = circleGO.AddComponent<OpenCircleTransition>();
            openCircleTransition.TransitionKit = this;
        }

        private IEnumerator Start()
        {
            yield return null;
            Initialize = true;
        }

        public void FadeScene(int sceneIndex, float duration, Color color)
        {
            if (isWorking) return;

            NextSceneIndex = sceneIndex;
            NextSceneName = string.Empty;
            fadeTransition.duration = duration / 2f;
            fadeTransition.image.material.SetColor("_Color", color);
            fadeTransition.ResetCounter();
            fadeTransition.gameObject.SetActive(true);

            StartCoroutine(fadeTransition.YieldTransition());
        }

        public void FadeScene(string sceneName, float duration, Color color)
        {
            if (isWorking) return;

            NextSceneName = sceneName;
            NextSceneIndex = -1;
            fadeTransition.image.material.SetColor("_Color", color);
            fadeTransition.duration = duration / 2f;
            fadeTransition.ResetCounter();
            fadeTransition.gameObject.SetActive(true);

            StartCoroutine(fadeTransition.YieldTransition());
        }

        public IEnumerator YieldFadeScreen(float duration, Color color)
        {
            fadeTransition.duration = duration / 2f;
            fadeTransition.image.material.SetColor("_Color", color);
            yield return fadeTransition.YieldTransition();
        }

        public IEnumerator YieldFadeOut(float duration, Color color)
        {
            fadeTransition.duration = duration;
            fadeTransition.image.material.SetColor("_Color", color);
            fadeTransition.ResetCounter();
            yield return fadeTransition.YieldFadeOut();
        }

        public IEnumerator YieldFadeIn(float duration, Color color)
        {
            fadeTransition.duration = duration;
            fadeTransition.image.material.SetColor("_Color", color);
            yield return fadeTransition.YieldFadeIn();
        }

        /// <summary>
        /// Fades the screen to a solid color, invokes the action (use it to swap GameObjects),
        /// then fades back in. No scene loading or lifecycle callbacks involved.
        /// </summary>
        public void FadeScreen(float fadeOutTime, float fadeInTime, Color color, System.Action onBlackScreen)
        {
            StartCoroutine(FadeScreenRoutine(fadeOutTime, fadeInTime, color, onBlackScreen));
        }

        private IEnumerator FadeScreenRoutine(float fadeOutTime, float fadeInTime, Color color, System.Action onBlackScreen)
        {
            fadeTransition.image.material.SetColor("_Color", color);
            fadeTransition.duration = fadeOutTime;
            fadeTransition.ResetCounter();
            yield return fadeTransition.YieldFadeOut();

            onBlackScreen?.Invoke();

            fadeTransition.duration = fadeInTime;
            yield return fadeTransition.YieldFadeIn();
        }

        public void OpenCircle(int levelLoad, float duration, Color color, string tag = null)
        {
            if (isWorking) return;
            NextSceneName = string.Empty;
            NextSceneIndex = levelLoad;

            openCircleTransition.followTag = tag;
            openCircleTransition.image.material.SetColor("_Color", color);
            openCircleTransition.duration = duration / 2f;
            openCircleTransition.ResetCounter();
            openCircleTransition.gameObject.SetActive(true);

            StartCoroutine(openCircleTransition.YieldTransition());
        }

        public void OpenCircle(string levelLoad, float duration, Color color, string tag = null)
        {
            if (isWorking) return;
            NextSceneName = levelLoad;
            NextSceneIndex = -1;

            openCircleTransition.followTag = tag;
            openCircleTransition.image.material.SetColor("_Color", color);
            openCircleTransition.duration = duration / 2f;
            openCircleTransition.ResetCounter();
            openCircleTransition.gameObject.SetActive(true);

            StartCoroutine(openCircleTransition.YieldTransition());
        }

        public void CompletedTransition()
        {
            OnTransitionEnd?.Invoke();
            isWorking = false;
            NextSceneName = string.Empty;
            NextSceneIndex = -1;
            OnTransitionStart = null;
            OnTransitionEnd = null;
            BeforeSceneLoad = null;
            AfterSceneLoad = null;
        }

        public void SetWorking(bool value)
        {
            isWorking = value;
        }
    }
}
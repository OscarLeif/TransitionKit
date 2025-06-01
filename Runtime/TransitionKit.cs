using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        public bool IsWorking => isWorking;

        public FadeTransition fadeTransition;
        public OpenCircleTransition openCircleTransition;

        public bool isWorking = false;

        public string NextSceneName;
        public int NextSceneIndex;

        public UnityEvent OnTransitionStart;
        public UnityEvent OnTransitionEnd;
        public UnityEvent BeforeSceneLoad;
        public UnityEvent AfterSceneLoad;

        public bool Initialize;

        private void Awake()
        {
            OnTransitionStart ??= new UnityEvent();
            OnTransitionEnd ??= new UnityEvent();
            BeforeSceneLoad ??= new UnityEvent();
            AfterSceneLoad ??= new UnityEvent();

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
        }

        public void CompletedTransition()
        {
            OnTransitionEnd?.Invoke();
            isWorking = false;
            NextSceneName = string.Empty;
            NextSceneIndex = -1;
            OnTransitionStart?.RemoveAllListeners();
            OnTransitionEnd?.RemoveAllListeners();
            BeforeSceneLoad?.RemoveAllListeners();
            AfterSceneLoad?.RemoveAllListeners();
        }

        public void SetWorking(bool value)
        {
            isWorking = value;
        }
    }
}

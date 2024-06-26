using AtaGames.TransitionKit.runtime;
using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace AtaGames.TransitionKit
{
    public class TransitionKit : MonoBehaviour
    {
        public static TransitionKit Get;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            if (Get == null)
            {
                GameObject gameObject = new GameObject(nameof(TransitionKit));
                Get = gameObject.gameObject.AddComponent<TransitionKit>();
                DontDestroyOnLoad(gameObject);
            }
        }

        public bool IsWorking => isWorking;

        public FadeTransition fadeTransition;
        public OpenCircleTransition openCircleTransition;

        public bool isWorking = false;

        public string NextSceneName;
        public int NextSceneIndex;

        public System.Action OnTransitionStart;
        public System.Action OnTransitionEnd;

        public System.Action BeforeSceneLoad;
        public System.Action AfterSceneLoad;

        public bool Initialize;

        private void Awake()
        {
            //FadeTransition GO
            GameObject FadeTransition = new GameObject(nameof(FadeTransition));
            FadeTransition.transform.parent = transform;
            fadeTransition = FadeTransition.AddComponent<FadeTransition>();
            fadeTransition.TransitionKit = this;

            //OpenCircle GO
            GameObject OpenCircleTransition = new GameObject(nameof(OpenCircleTransition));
            OpenCircleTransition.transform.parent = transform;
            openCircleTransition = OpenCircleTransition.AddComponent<OpenCircleTransition>();
            openCircleTransition.TransitionKit = this;
        }

        private IEnumerator Start()
        {
            yield return null;
            Initialize = true;
        }

        public void FadeScene(int sceneIndex, float duration, Color color)
        {
            if (isWorking) { Debug.Log("This should not happen check"); return; }
            if (fadeTransition == null) { }

            NextSceneIndex = sceneIndex;
            NextSceneName = string.Empty;
            fadeTransition.duration = duration / 2;
            fadeTransition.ResetCounter();
            fadeTransition.gameObject.SetActive(true);

            OnTransitionStart?.Invoke();
            //StartCoroutine(fadeTransition.LoadSceneRoutine());
        }

        public void FadeScene(string sceneName, float duration, Color color)
        {
            if (isWorking) { return; }
            if (fadeTransition == null) { }
            NextSceneName = sceneName;
            NextSceneIndex = -1;

            //fadeTransition.image.color = color;//Vertex Color
            fadeTransition.image.material.SetColor("_Color", color);//Shader Color
            fadeTransition.duration = duration / 2f;
            fadeTransition.ResetCounter();
            fadeTransition.gameObject.SetActive(true);
            //StartCoroutine(fadeTransition.LoadScene());
        }

        public void FadeScreen(float duration, Color color)
        {
            if (isWorking) { return; }
            NextSceneName = string.Empty;
            NextSceneIndex = -1;
            fadeTransition.ResetCounter();
        }

        public IEnumerator YieldFadeScreen(float duration, Color color)
        {
            //if Next scene is not setup 
            fadeTransition.duration = duration / 2f;//InOut that's why divided by 2
            fadeTransition.image.material.SetColor("_Color", color);//Shader Color
            yield return fadeTransition.YieldTransition();
            yield return null;
        }

        public void OpenCircle(int levelLoad, float duration, Color color, string tag = null)
        {
            if (isWorking) return;
            NextSceneName = string.Empty;
            NextSceneIndex = levelLoad;

            openCircleTransition.followTag = tag;
            openCircleTransition.image.material.SetColor("_Color", color);//Shader Color
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
            openCircleTransition.image.material.SetColor("_Color", color);//Shader Color
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
            OnTransitionStart = OnTransitionEnd = null;
            BeforeSceneLoad = AfterSceneLoad = null;
        }

        // Method to allow specific classes to set the working status
        public void SetWorking(bool value)
        {
            isWorking = value;
        }
    }
}
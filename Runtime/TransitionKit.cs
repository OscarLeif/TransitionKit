using AtaGames.TransitionKit.runtime;
using System;
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
                Get = gameObject.AddComponent<TransitionKit>();
                DontDestroyOnLoad(gameObject);
            }
        }

        public FadeTransition fadeTransition;

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
            GameObject FadeTransition = new GameObject(nameof(FadeTransition));
            FadeTransition.transform.parent = transform;
            fadeTransition = FadeTransition.AddComponent<FadeTransition>();
            fadeTransition.TransitionKit = this;
        }

        private void Start()
        {
            Initialize = true;
        }

        public void FadeScene(int sceneIndex, float duration, Color color)
        {
            if (isWorking) { return; }
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

        public void CompletedTransition()
        {
            OnTransitionEnd?.Invoke();
            isWorking = false;
            NextSceneName = string.Empty;
            NextSceneIndex = -1;
            OnTransitionStart = OnTransitionEnd = null;
            BeforeSceneLoad = AfterSceneLoad = null;
        }

        public void OpenCircle(string levelLoad, float v, Color black, string player)
        {
            Debug.Log("OPEN CIRCLE IS NOT PRESENT USE FADE ");
            FadeScene(levelLoad, v, black);
        }

        public bool IsWorking => isWorking;
        
    }
}
using AtaGames.TransitionKit.runtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        private FadeTransition fadeTransition;

        public bool isWorking = false;

        public string NextScene;

        private void Awake()
        {
            GameObject FadeTransition = new GameObject(nameof(FadeTransition));
            FadeTransition.transform.parent = transform;
            fadeTransition = FadeTransition.AddComponent<FadeTransition>();
            fadeTransition.TransitionKit = this;
        }

        public void FadeScene(string sceneName, float duration, Color color)
        {
            if (isWorking) { return; }
            if (fadeTransition == null) { }
            NextScene = sceneName;
            fadeTransition.ResetCounter();
            fadeTransition.gameObject.SetActive(true);

        }
    }
}
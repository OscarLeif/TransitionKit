using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtaGames.TransitionKit
{
    public abstract class TransitionScene
    {
        public string id = string.Empty;

        public int nextSceneIndex = -1;

        public string nextSceneName = string.Empty;

        public float transitionTime = 0.5f;

        public Material MaterialRef;

        public abstract Shader shaderForTransition();

        public abstract Mesh meshForDisplay();

        public abstract Texture2D textureForDisplay();

        public abstract IEnumerator onScreenObscured(TransitionKit transitionKit);

        public abstract Task onScreenObscuredTask(TransitionKit transitionKit);

        public void SetDuration(float duration)
        {
            this.transitionTime = duration;
        }

        //Coroutine Version
        public virtual IEnumerator LoadScene()
        {
            TransitionKit.BeforeSceneLoad?.Invoke();
            AsyncOperation asyncLoad = null;
            // Wait until the asynchronous scene fully loads
            if (string.IsNullOrEmpty(nextSceneName) == false)
            {
                asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, TransitionKit.LoadMode);
            }
            else if (nextSceneIndex >= 0)
            {
                asyncLoad = SceneManager.LoadSceneAsync(nextSceneIndex, TransitionKit.LoadMode);
            }

            if (asyncLoad != null)
            {
                while (asyncLoad.isDone == false)
                {
                    yield return null;
                }
            }
            TransitionKit.AfterSceneLoad?.Invoke();
            yield return Yielders.GetRealTime(TransitionKit.DelayAfterLoad);//What this does ?
        }

        public virtual async void LoadSceneAsync()
        {
            TransitionKit.BeforeSceneLoad?.Invoke();
            AsyncOperation asyncOperation = null;
            if (string.IsNullOrEmpty(nextSceneName) == false)
            {
                asyncOperation = SceneManager.LoadSceneAsync(nextSceneName, TransitionKit.LoadMode);
                asyncOperation.allowSceneActivation = true;
            }
            else if (nextSceneIndex >= 0)
            {
                asyncOperation = SceneManager.LoadSceneAsync(nextSceneIndex, TransitionKit.LoadMode);
            }
            if (asyncOperation != null)
            {
                while (asyncOperation.isDone == false)
                {
                    await Task.Yield();
                }
            }
            TransitionKit.AfterSceneLoad?.Invoke();
        }
    }
}

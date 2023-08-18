using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit.runtime
{
    /// <summary>
    /// <see cref="TransitionKit"/>
    /// </summary>
    public class FadeTransition : MonoBehaviour, ITransition
    {
        public TransitionKit TransitionKit;

        public Canvas canvas;
        public Image image;

        public TransitionState transitionState;

        public LoadState loadState;

        public float duration = 1f;
        public float holdDuration = 0.5f;

        [System.NonSerialized] private float counterTransition;
        [System.NonSerialized] private float counterHold;
        [System.NonSerialized] private float loadingProgressTarget;

        private AsyncOperation loading;
        private bool CoroutineWorking;

        public void ResetCounter()
        {
            transitionState = TransitionState.StateIn;
            counterTransition = 0;
            counterHold = 0;
            loadingProgressTarget = 0;
            image.material.SetFloat(TransitionKitConstants._Progress, 0);
        }

        private void Awake()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;//High number to make sure it's on top of everything

            image = gameObject.AddComponent<Image>();
            Material fader = new Material(Shader.Find(TransitionKitConstants.FadeShader));
            image.material = fader;
            Texture2D blackTex = new Texture2D(1, 1);
            blackTex.name = "FadeTexture";
            blackTex.SetPixel(0, 0, Color.clear);
            blackTex.Apply();
            image.material.mainTexture = blackTex;
            ResetCounter();
        }

        private IEnumerator Start()
        {
            yield return null;
            DisableGameObject();
        }

        public async void Update()
        {
            if (CoroutineWorking) return;

            TransitionKit.isWorking = true;

            if (transitionState == TransitionState.StateIn)
            {
                if (TransitionLerp(-0.1f, 1.1f, false))
                {
                    counterHold = 0;
                    transitionState = TransitionState.Hold;
                    LoadScene();
                }
            }
            if (transitionState == TransitionState.Hold)
            {
                AsyncOperation asyncOperation = null;
                if (string.IsNullOrEmpty(TransitionKit.NextSceneName) == false)
                {
                    asyncOperation = SceneManager.LoadSceneAsync(TransitionKit.NextSceneName);
                }
                else if (TransitionKit.NextSceneIndex >= 0)
                {
                    asyncOperation = SceneManager.LoadSceneAsync(TransitionKit.NextSceneIndex);
                }

                while (!asyncOperation.isDone)
                {
                    //loadingProgressTarget = asyncOperation.progress;
                    await Task.Yield();
                }
                transitionState = TransitionState.HoldDelay;
                counterHold = 0;

            }
            else if (transitionState == TransitionState.HoldDelay)
            {
                if (HoldTime())
                {
                    transitionState = TransitionState.StateOut;
                }
            }
            else if (transitionState == TransitionState.StateOut)
            {

                TransitionLerp(1.1f, -0.1f);
            }
        }

        private void LoadScene()
        {
            TransitionKit.BeforeSceneLoad?.Invoke();

            //When Using the Async Load Scene.
            //It doesn't work properly on Android.
            if (string.IsNullOrEmpty(TransitionKit.NextSceneName) == false)
            {
                SceneManager.LoadScene(TransitionKit.NextSceneName);
            }
            else if (TransitionKit.NextSceneIndex >= 0)
            {
                SceneManager.LoadScene(TransitionKit.NextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No Valid Scene To Load");
            }
            //we could put a delay here.

            TransitionKit.AfterSceneLoad?.Invoke();
        }

        public IEnumerator YieldTransition()
        {
            CoroutineWorking = true;
            //disable the update
            gameObject.SetActive(true);
            //this.transitionState = TransitionState.StateOut;

            TransitionKit.isWorking = true;
            TransitionKit.OnTransitionStart?.Invoke();
            float start = -0.1f;
            float end = 1.1f;
            //Lerp In
            float timeElapsed = 0f;
            float stepDuration = duration / 2;//Lerp In and Lerp Out

            while (timeElapsed < stepDuration)
            {
                counterTransition = Mathf.Lerp(start, end, timeElapsed / stepDuration);
                timeElapsed += Time.unscaledDeltaTime;
                image.material.SetFloat(TransitionKitConstants._Progress, counterTransition);
                yield return null;
            }
            image.material.SetFloat(TransitionKitConstants._Progress, end);

            TransitionKit.BeforeSceneLoad?.Invoke();

            if (TransitionKit.NextSceneIndex >= 0)
            {
                loading = SceneManager.LoadSceneAsync(TransitionKit.NextSceneIndex);
            }
            else if (string.IsNullOrEmpty(TransitionKit.NextSceneName) == false)
            {
                loading = SceneManager.LoadSceneAsync(TransitionKit.NextSceneName);
            }

            if (loading != null)
            {
                while (loading.isDone == false)
                {
                    Debug.Log("Loading");
                    yield return null;
                }
            }

            Debug.Log("Fadeout");
            yield return new WaitForSecondsRealtime(holdDuration);

            TransitionKit.AfterSceneLoad?.Invoke();

            timeElapsed = 0f;
            start = 1.1f;
            end = -0.1f;

            yield return null;
            //Lerp Out
            while (timeElapsed < stepDuration)
            {
                Debug.Log("Fadeout");
                counterTransition = Mathf.Lerp(start, end, timeElapsed / stepDuration);
                timeElapsed += Time.unscaledDeltaTime;
                image.material.SetFloat(TransitionKitConstants._Progress, counterTransition);
                yield return null;
            }
            image.material.SetFloat(TransitionKitConstants._Progress, end);

            TransitionKit.CompletedTransition();
            CoroutineWorking = false;
            Debug.Log("Coroutine Done!");
            gameObject.SetActive(false);
            //Hold
        }


        private bool TransitionLerp(float start, float end, bool turnOff = true)
        {
            float value = TransitionUtils.LerpUnscaled(start, end, duration, ref counterTransition, out bool complete);
            if (image != null && image.material != null)
            {
                image.material.SetFloat(TransitionKitConstants._Progress, value);
            }
            if (complete && turnOff)
            {
                gameObject.SetActive(false);
                TransitionKit.CompletedTransition();
            }
            return complete;
        }

        private bool HoldTime()
        {
            counterHold += Time.unscaledDeltaTime;
            if (counterHold >= holdDuration)
            {
                counterHold = 0;
                return true;
            }
            return false;
        }


        private void DisableGameObject()
        {
            gameObject.SetActive(false);
            TransitionKit.CompletedTransition();
        }

    }
}

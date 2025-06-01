using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit
{
    public class FadeTransition : MonoBehaviour, ITransition
    {
        public TransitionKit TransitionKit;

        public Canvas canvas;
        public Image image;

        public TransitionState transitionState;

        public float duration = 1f;
        public float holdDuration = 0.5f;

        private float counterTransition;

        private AsyncOperation loading;
        private bool CoroutineWorking;

        public void ResetCounter()
        {
            transitionState = TransitionState.StateIn;
            counterTransition = 0;
            image.material.SetFloat(TransitionKitConstants._Progress, 0);
        }

        private void Awake()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

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
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            loading = null;
        }

        public IEnumerator YieldTransition()
        {
            CoroutineWorking = true;
            gameObject.SetActive(true);

            TransitionKit.isWorking = true;
            TransitionKit.OnTransitionStart?.Invoke();
            TransitionKit.OnTransitionStart.RemoveAllListeners();

            const float FadeInStart = -0.1f;
            const float FadeInEnd = 1.1f;
            float timeElapsed = 0f;
            float stepDuration = duration / 2f;

            while (timeElapsed < stepDuration)
            {
                counterTransition = Mathf.Lerp(FadeInStart, FadeInEnd, timeElapsed / stepDuration);
                timeElapsed += Time.unscaledDeltaTime;
                image.material.SetFloat(TransitionKitConstants._Progress, counterTransition);
                yield return null;
            }
            image.material.SetFloat(TransitionKitConstants._Progress, FadeInEnd);

            TransitionKit.BeforeSceneLoad?.Invoke();
            TransitionKit.BeforeSceneLoad.RemoveAllListeners();

            if (TransitionKit.NextSceneIndex >= 0)
                loading = SceneManager.LoadSceneAsync(TransitionKit.NextSceneIndex);
            else if (!string.IsNullOrEmpty(TransitionKit.NextSceneName))
                loading = SceneManager.LoadSceneAsync(TransitionKit.NextSceneName);

            if (loading != null)
            {
                while (!loading.isDone)
                    yield return null;
            }

            yield return new WaitForSecondsRealtime(holdDuration);

            TransitionKit.AfterSceneLoad?.Invoke();
            TransitionKit.AfterSceneLoad.RemoveAllListeners();

            timeElapsed = 0f;
            const float FadeOutStart = 1.1f;
            const float FadeOutEnd = -0.1f;

            while (timeElapsed < stepDuration)
            {
                counterTransition = Mathf.Lerp(FadeOutStart, FadeOutEnd, timeElapsed / stepDuration);
                timeElapsed += Time.unscaledDeltaTime;
                image.material.SetFloat(TransitionKitConstants._Progress, counterTransition);
                yield return null;
            }
            image.material.SetFloat(TransitionKitConstants._Progress, FadeOutEnd);

            TransitionKit.CompletedTransition();
            CoroutineWorking = false;
            gameObject.SetActive(false);
        }
    }
}
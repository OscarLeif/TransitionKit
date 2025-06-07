using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit
{
    public class OpenCircleTransition : MonoBehaviour, ITransition
    {
        public TransitionKit TransitionKit;

        public Canvas canvas;
        public Image image;

        public float duration = 1f;
        public float holdDuration = 0.5f;

        [System.NonSerialized] private float counterTransition;
        [System.NonSerialized] private float counterHold;
        [System.NonSerialized] private float loadingProgressTarget;

        private AsyncOperation loading;
        private bool CoroutineWorking;

        [SerializeField] public bool FollowMouse = false;
        public string followTag = string.Empty;
        private Transform followTr = null;//Used only for the followTag

        //Shader Properties for Open Circle
        public static readonly int offsetID = Shader.PropertyToID("_Offset");

        private void Awake()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;//High number to make sure it's on top of everything

            image = gameObject.AddComponent<Image>();
            Material fader = new Material(Shader.Find(TransitionKitConstants.CircleCutoutShader));
            image.material = fader;
            Texture2D blackTex = new Texture2D(1, 1);
            blackTex.name = "OpenCirceTexture";
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

        private void SetCenter()
        {
            if (string.IsNullOrEmpty(followTag) == false)
            {
                if (followTr == null)
                {
                    GameObject go = GameObject.FindGameObjectWithTag(followTag);
                    if (go != null)
                    {
                        followTr = go.transform;
                    }
                }
                if (Camera.main != null && followTr != null)
                {
                    Vector2 pos = Camera.main.WorldToViewportPoint(followTr.position);
                    this.image.material.SetVector(offsetID, pos);
                }
            }
            else
            {
                //Default Center (Center Screen)
                this.image.material.SetVector(offsetID, new Vector2(0.5f, 0.5f));
            }
        }        

        public void ResetCounter()
        {
            counterTransition = 0;
            counterHold = 0;
            loadingProgressTarget = 0;
            image.material.SetFloat(TransitionKitConstants._Progress, 0);
        }

        public void DisableGameObject()
        {
            gameObject.SetActive(false);
            followTag = string.Empty;
            followTr = null;
            TransitionKit.CompletedTransition();
        }

        public IEnumerator YieldTransition()
        {
            if (CoroutineWorking)
                yield break;

            CoroutineWorking = true;
            gameObject.SetActive(true);

            TransitionKit.isWorking= true;

            Utils.FireAndClearEvent(TransitionKit.OnTransitionStart);
            const float FadeInStart = -0.1f;
            const float FadeInEnd = 1.1f;
            float timeElapsed = 0f;
            float stepDuration = duration / 2f;

            while (timeElapsed < stepDuration)
            {
                counterTransition = Mathf.Lerp(FadeInStart, FadeInEnd, timeElapsed / stepDuration);
                timeElapsed += Time.unscaledDeltaTime;
                image.material.SetFloat(TransitionKitConstants._Progress, counterTransition);
                SetCenter();
                yield return null;
            }
            image.material.SetFloat(TransitionKitConstants._Progress, FadeInEnd);

            Utils.FireAndClearEvent(TransitionKit.BeforeSceneLoad);

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

            Utils.FireAndClearEvent(TransitionKit.AfterSceneLoad);

            timeElapsed = 0f;
            const float FadeOutStart = 1.1f;
            const float FadeOutEnd = -0.1f;

            while (timeElapsed < stepDuration)
            {
                SetCenter();
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

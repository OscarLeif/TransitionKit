using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit.runtime
{
    public class OpenCircleTransition : MonoBehaviour, ITransition
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

        public void Update()
        {
            if (CoroutineWorking) return;

            TransitionKit.isWorking = true;
            SetCenter();

            if (transitionState == TransitionState.StateIn)
            {
                if (TransitionLerp(-0.1f, 1.1f, false))
                {
                    counterHold = 0;
                    transitionState = TransitionState.Hold;
                    LoadScene();
                }
            }
            else if (transitionState == TransitionState.Hold)
            {
                counterHold += Time.unscaledDeltaTime;
                if (counterHold >= holdDuration)
                {
                    transitionState = TransitionState.StateOut;
                }
            }
            else if (transitionState == TransitionState.StateOut)
            {
                TransitionLerp(1.1f, -0.1f);
            }
        }

        public IEnumerator YieldTransition()
        {
            Debug.LogWarning("No Implement");
            yield return null;
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
                Vector2 pos = Camera.main.WorldToViewportPoint(followTr.position);
                this.image.material.SetVector(offsetID, pos);
            }
            else
            {
                //Default Center (Center Screen)
                this.image.material.SetVector(offsetID, new Vector2(0.5f, 0.5f));
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
                followTag = string.Empty;
                followTr = null;
                TransitionKit.CompletedTransition();
            }
            return complete;
        }

        public void ResetCounter()
        {
            transitionState = TransitionState.StateIn;
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
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit.runtime
{
    /// <summary>
    /// <see cref="TransitionKit"/>
    /// </summary>
    public class FadeTransition : MonoBehaviour
    {
        public TransitionKit TransitionKit;

        public Canvas canvas;
        public Image image;

        public TransitionState transitionState;


        public float duration = 1f;

        [System.NonSerialized] private float counterTransition;
        [System.NonSerialized] private float counterHold;

        private void Awake()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;//High number to make sure it's on top of everything

            image = gameObject.AddComponent<Image>();
            Material fader = new Material(Shader.Find(TransitionKitConstants.FadeShader));
            image.material = fader;
            Texture2D blackTex = new Texture2D(1, 1);
            blackTex.SetPixel(0, 0, Color.clear);
            blackTex.Apply();
            image.material.mainTexture = blackTex;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {

        }

        private void Update()
        {
            if (transitionState == TransitionState.StateIn)
            {
                if (TransitionLerp(-0.1f, 1.1f, false))
                {
                    transitionState = TransitionState.Hold;
                    SceneManager.LoadSceneAsync(TransitionKit.NextScene);
                }
            }
            if (transitionState == TransitionState.Hold)
            {
                counterHold += Time.unscaledDeltaTime;
                if (counterHold > 0.1f)
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
            if (string.IsNullOrEmpty(TransitionKit.NextScene))
                SceneManager.LoadSceneAsync(TransitionKit.NextScene);
        }

        public void ResetCounter()
        {
            TransitionKit.isWorking = true;
            this.counterHold = 0;
            this.counterTransition = 0;
            this.transitionState = TransitionState.StateIn;
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
                TransitionKit.isWorking = false;
            }
            return complete;
        }
    }

    public enum TransitionState
    {
        StateIn, Hold, StateOut
    }
}

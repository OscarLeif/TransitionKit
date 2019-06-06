using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prime31.TransitionKit
{
    public class CircleOpeningTransition : TransitionKitDelegate
    {
        public float duration = 2f;
        public int nextScene = -1;
        public string nextSceneName = "";
        public Color backgroundColor = Color.clear;
        public float smoothness = 0.025f;
        public GameObject targetCenter;

        public float CircleOpeneingDelay;

        private float ratio;

        public Shader shaderForTransition()
        {
            return Shader.Find("prime[31]/Transitions/CircleOpen");
        }

        public Mesh meshForDisplay()
        {
            return null;
        }

        public Texture2D textureForDisplay()
        {
            //var screenSnapshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, false);
            //return null;
            var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);

            // set the pixel values
            texture.SetPixel(0, 0, backgroundColor);
            texture.SetPixel(1, 0, backgroundColor);
            texture.SetPixel(0, 1, backgroundColor);
            texture.SetPixel(1, 1, backgroundColor);

            // Apply all SetPixel calls
            texture.Apply();

            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.clear);
            tex.Apply();

            //material.mainTexture = tex;
            texture = tex;

            return texture;
        }

        public IEnumerator onScreenObscured(TransitionKit transitionKit)
        {
            transitionKit.material.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b);
            transitionKit.material.SetFloat("_AspectRatio", (float)Screen.width / (float)Screen.height);
            AsyncOperation asyncLeveLoad = null;
            if (targetCenter)
            {
                transitionKit.StartCoroutine(FollowTargetPosition(transitionKit));
            }
            //Circle open to close animation
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration / 3, false));
            //Load a new Scene by index or name string (heavy)
            if (nextScene >= 0)
            {
                asyncLeveLoad = SceneManager.LoadSceneAsync(nextScene);
            }
            // Load Scene by name
            if (nextSceneName != null && !nextSceneName.Equals(""))
            {
                if (Application.CanStreamedLevelBeLoaded(nextSceneName))
                {
                    asyncLeveLoad = SceneManager.LoadSceneAsync(nextSceneName);
                }
            }

            //Let the screen close for 
            yield return new WaitForSecondsRealtime(duration);

            if (asyncLeveLoad != null)
            {
                while (!asyncLeveLoad.isDone)
                {
                    yield return null;
                }
            }

            // Open the Circle a new scene should be loaded, if not scene will be the same
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration / 3, true));
        }

        public IEnumerator FollowTargetPosition(TransitionKit transitionKit)
        {
            while (true)
            {
                if (this.targetCenter == null)
                {
                    transitionKit.material.SetFloat("_X", 0.5f);
                    transitionKit.material.SetFloat("_Y", 0.5f);
                }
                else if (this.targetCenter)
                {
                    transitionKit.material.SetFloat("_X", Camera.main.WorldToViewportPoint(targetCenter.transform.position).x);
                    transitionKit.material.SetFloat("_Y", Camera.main.WorldToViewportPoint(targetCenter.transform.position).y);
                }
                yield return null;
            }
        }
    }
}


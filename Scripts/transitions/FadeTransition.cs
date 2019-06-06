using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
    public class FadeTransition : TransitionKitDelegate
    {
        public Color fadeToColor = Color.black;
        public float duration = 0.5f;
        /// <summary>
        /// the effect looks best when it pauses before fading back. When not doing a scene-to-scene transition you may want
        /// to pause for a breif moment before fading back.
        /// </summary>
        public float fadedDelay = 1f;

        public int nextScene = -1;
        public string nextSceneName;

        public System.Action actionDelegate;


        #region TransitionKitDelegate

        public Shader shaderForTransition()
        {
            return Shader.Find("prime[31]/Transitions/Fader");
        }

        public Mesh meshForDisplay()
        {
            return null;
        }

        public Texture2D textureForDisplay()
        {
            /*Texture2D texture = null;
            if ((Texture2D)Resources.Load("BlackSquare", typeof(Texture2D)) != null)
            {
                texture = (Texture2D)Resources.Load("BlackSquare", typeof(Texture2D));
                makeTextureTransparent(texture);
            }
            return texture;*/
            var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

            // set the pixel values
            texture.SetPixel(0, 0, Color.black);
            texture.SetPixel(1, 0, Color.black);
            texture.SetPixel(0, 1, Color.black);
            texture.SetPixel(1, 1, Color.black);

            // Apply all SetPixel calls
            texture.Apply();

            return texture;
        }

        public IEnumerator onScreenObscured(TransitionKit transitionKit)
        {
            //transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
            transitionKit.material.color = new Color(fadeToColor.r, fadeToColor.g, fadeToColor.b, 0);
            //Debug.Log("Set Color to shader");
            float actionTime = duration / 2;
            AsyncOperation asyncLevelLoad = null;

            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration, true));
            //Debug.Log("Fade In completed");

            if (this.actionDelegate != null)
                this.actionDelegate.Invoke();

            if (nextScene >= 0)
            {
                asyncLevelLoad = SceneManager.LoadSceneAsync(nextScene);
                //Debug.Log("Load new Scene by Index: " + nextScene);
            }

            if (nextSceneName != null && !nextSceneName.Equals(""))
            {
                asyncLevelLoad = SceneManager.LoadSceneAsync(nextSceneName);
            }

            if (asyncLevelLoad != null)
            {
                while (!asyncLevelLoad.isDone)
                    yield return null;
            }

            //Delay for animation
            yield return new WaitForSecondsRealtime(actionTime);

            //Debug.Log("Start Fadeout");
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration));
            //Debug.Log("Completed Fadeout");

        }
        #endregion

    }
}

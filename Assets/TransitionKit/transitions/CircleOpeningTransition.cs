using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prime31.TransitionKit
{
    public class CircleOpeningTransition : TransitionKitDelegate
    {
        public float duration = 1.0f;
        public int nextScene = -1;
        public Color backgroundColor = Color.black;
        public float smoothness = 0.3f;
        public GameObject targetCenter;

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
            return null;
        }

        public IEnumerator onScreenObscured(TransitionKit transitionKit)
        {
            transitionKit.StartCoroutine(FollowTargetPosition(transitionKit));

            transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
            transitionKit.material.color = backgroundColor;
            transitionKit.material.SetFloat("_Smoothness", smoothness);
            transitionKit.material.SetFloat("_Ratio", (float)Screen.width / (float)Screen.height);

            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            // this does the zoom/rotation
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration, true));

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.waitForLevelToLoad(nextScene));

            // now that the new scene is loaded we zoom the mask back out
            transitionKit.makeTextureTransparent();

            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(duration, false));

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
                    transitionKit.material.SetFloat("_Y", Camera.main.ScreenToViewportPoint(targetCenter.transform.position).y);
                }
                yield return null;
            }
        }
    }
}


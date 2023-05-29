using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AtaGames.TransitionKit.runtime
{
    public class OpenCircleTransition : TransitionScene
    {
        public Color backgroundColor = Color.black;

        public override Mesh meshForDisplay() { return null; }

        public static readonly int offsetID = Shader.PropertyToID("_Offset");

        [System.NonSerialized] public Transform followTransform;
        [System.NonSerialized] public string TAG;
        [System.NonSerialized] private Material material;

        public override IEnumerator onScreenObscured(TransitionKit transitionKit)
        {
            TransitionKit.IsWorking = true;
            transitionKit.material.color = backgroundColor;
            this.material = transitionKit.material;
            SetupFollowTarget(transitionKit);
            float actionTime = this.transitionTime / 2f;
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(actionTime, false));
            yield return LoadScene();
            yield return Yielders.GetRealTime(TransitionKit.DelayAfterLoad);
            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(actionTime, true));
            TransitionKit.IsWorking = false;
        }

        private void SetupFollowTarget(TransitionKit transitionKit)
        {
            if (!string.IsNullOrEmpty(TAG))
            {
                transitionKit.StartCoroutine(FollowTag(TAG));
            }
            else if (followTransform != null)
            {
                transitionKit.StartCoroutine(FollowTransform(followTransform));
            }
        }

        public override Task onScreenObscuredTask(TransitionKit transitionKit)
        {
            throw new System.NotImplementedException();
        }

        public override Shader shaderForTransition()
        {
            return Shader.Find(TransitionKitConstants.CircleCutoutShader);
        }

        public override Texture2D textureForDisplay()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.clear);
            tex.Apply();
            return tex;
        }

        /// Like the Mouse, but Here the Transform could be missed.
        private IEnumerator FollowTransform(Transform transform)
        {
            while (TransitionKit.IsWorking)
            {
                if (Camera.main && transform != null)
                {
                    Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
                    this.material.SetVector(offsetID, pos);
                }
                else
                {
                    //Just Move to the Center.
                    this.material.SetVector(offsetID, new Vector2(0.5f, 0.5f));
                }
                yield return null;
            }
        }

        private IEnumerator FollowTag(string TAG)
        {
            GameObject go = null;

            while (TransitionKit.IsWorking)
            {
                if (go == null) { go = GameObject.FindGameObjectWithTag(TAG); }
                if (Camera.main && go != null)
                {
                    Vector2 pos = Camera.main.WorldToViewportPoint(go.transform.position);
                    this.material.SetVector(offsetID, pos);
                }
                else
                {
                    this.material.SetVector(offsetID, new Vector2(0.5f, 0.5f));
                }
                yield return null;
            }
            yield return null;
        }

        private IEnumerator FollowMousePosition()
        {
            while (TransitionKit.IsWorking)
            {
                Vector2 mousePosition = Input.mousePosition;//Pixel Coordinates                
                Vector2 ViewPort = Camera.main.ScreenToViewportPoint(mousePosition);
                //I think the Rect is Upper Left 0, 0 down right 1,1

                this.material.SetVector(offsetID, ViewPort);
                Debug.Log("Mouse Position Update");
                yield return null;
            }
        }
    }
}

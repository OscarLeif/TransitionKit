using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AtaGames.TransitionKit
{
    public class FadeTransition : TransitionScene
    {
        public Color fadeToColor = Color.black;

        public override Mesh meshForDisplay() { return null; }

        public override IEnumerator onScreenObscured(TransitionKit transitionKit)
        {
            TransitionKit.IsWorking = true;
            transitionKit.material.color = fadeToColor;
            float actionTime = this.transitionTime / 2f;

            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(actionTime, false));

            yield return LoadScene();

            yield return Yielders.GetRealTime(TransitionKit.DelayAfterLoad);

            yield return transitionKit.StartCoroutine(transitionKit.tickProgressPropertyInMaterial(actionTime, true));

            TransitionKit.IsWorking = false;
        }

        public override async Task onScreenObscuredTask(TransitionKit transitionKit)
        {
            TransitionKit.IsWorking = true;
            transitionKit.material.color = fadeToColor;
            float actionTime = this.transitionTime / 2f;

            await transitionKit.tickProgressPropertyInMaterialTask(actionTime, false);
        }

        public override Shader shaderForTransition()
        {
            return Shader.Find(TransitionKitConstants.FadeShader);
        }

        public override Texture2D textureForDisplay()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.clear);
            tex.Apply();
            return tex;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit
{
    /// <summary>
    /// Template for Creating Fading effects
    /// Need to review what is really using.
    /// </summary>
    public interface TransitionKitDelegate
    {
        Shader shaderForTransition();

        Mesh meshForDisplay();

        Texture2D textureForDisplay();

        IEnumerator onScreenObscured(TransitionKit transitionKit);
    }
}
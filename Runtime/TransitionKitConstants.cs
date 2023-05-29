using UnityEngine;

namespace AtaGames.TransitionKit
{
    public class TransitionKitConstants
    {
        //Shader Names
        public const string FadeShader = "prime[31]/Transitions/Fader";
        public const string CircleCutoutShader = "Circle";

        //Common ID to Transition Shaders
        public static int _Progress = Shader.PropertyToID(nameof(_Progress));
    }
}
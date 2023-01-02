using UnityEngine;

namespace AtaGames.TransitionKit
{
    public partial class TransitionKit : MonoBehaviour
    {
        //TransitionKit.FadeScene
        public static void FadeScene(string sceneName, float actionTime, Color color)
        {
            var fadeScene = new FadeTransition()
            {
                nextSceneName = sceneName,
                transitionTime = actionTime,
                fadeToColor = color
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(fadeScene));
        }    

        public static void FadeScene(int sceneIndex, float actionTime, Color color)
        {
            var fadeScene = new FadeTransition()
            {
                nextSceneIndex = sceneIndex,
                transitionTime = actionTime,
                fadeToColor = color
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(fadeScene));
        }        
    }
}
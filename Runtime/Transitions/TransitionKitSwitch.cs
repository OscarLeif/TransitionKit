using AtaGames.TransitionKit.runtime;
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

        public static void OpenCircle(int sceneIndex, float actionTime, Color color, string TAG)
        {
            var openCircleTransition = new OpenCircleTransition()
            {
                nextSceneIndex = sceneIndex,
                transitionTime = actionTime,
                backgroundColor = color,
                TAG = TAG
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(openCircleTransition));
        }

        public static void OpenCircle(string sceneName, float actionTime, Color color, string TAG)
        {
            var openCircleTransition = new OpenCircleTransition()
            {
                nextSceneName = sceneName,
                transitionTime = actionTime,
                backgroundColor = color,
                TAG = TAG
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(openCircleTransition));
        }

        public static void OpenCircle(int sceneIndex, float actionTime, Color color, Transform follow)
        {
            var openCircleTransition = new OpenCircleTransition()
            {
                nextSceneIndex = sceneIndex,
                transitionTime = actionTime,
                backgroundColor = color,
                followTransform = follow
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(openCircleTransition));
        }

        public static void OpenCircle(string sceneName, float actionTime, Color color, Transform follow)
        {
            var openCircleTransition = new OpenCircleTransition()
            {
                nextSceneName = sceneName,
                transitionTime = actionTime,
                backgroundColor = color,
                followTransform = follow
            };
            TransitionKit.Instance.StartCoroutine(TransitionKit.Instance.TransitionWithDelegate(openCircleTransition));
        }
    }
}
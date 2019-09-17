using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prime31.TransitionKit
{
    public class ReplaceScene : MonoBehaviour
    {
        public Texture2D maskTexture;
        [Tooltip("If UI Visible is True, transition is not working, False transition is working")]
        private bool _isUiVisible = true;

        public bool UiVisible
        {
            get { return _isUiVisible; }
        }

        private static ReplaceScene inst;

        public static ReplaceScene Inst
        {
            get
            {
                if (inst == null)
                {
                    inst = GameObject.FindObjectOfType<ReplaceScene>();
                    if (inst == null)
                    {
                        GameObject singleton = new GameObject(typeof(ReplaceScene).Name);
                        inst = singleton.AddComponent<ReplaceScene>();
                        inst.name = typeof(ReplaceScene).Name;
                    }
                }
                return inst;
            }
        }

        private void Awake()
        {
            if (inst == null)
            {
                inst = Inst;
                DontDestroyOnLoad(base.gameObject);
            }
            else
            {
                Destroy(base.gameObject);
            }
        }

        public static void FadeScene(int sceneIndex, float secondsToWait = 1.0f)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }

            var fader = new FadeTransition()
            {
                nextScene = sceneIndex,
                fadeToColor = Color.black,
                duration = secondsToWait,
                //imageTexture = ReplaceScene.Inst.maskTexture
            };
            TransitionKit.instance.transitionWithDelegate(fader);
        }

        public static void FadeScene(string sceneName, float secondsToWait = 1f)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }
            var fader = new FadeTransition()
            {
                nextSceneName = sceneName,
                fadeToColor = Color.black,
                duration = secondsToWait,
                //imageTexture = ReplaceScene.Inst.maskTexture
            };
            TransitionKit.instance.transitionWithDelegate(fader);
        }

        public static void FadeScene(string sceneName, float secondsAction = 1f, Action actionBeforeLoadScene = null)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }

            var fader = new FadeTransition()
            {
                nextSceneName = sceneName,
                fadeToColor = Color.black,
                duration = secondsAction,
                //imageTexture = ReplaceScene.Inst.maskTexture
                actionCallbackBeforeLoadScene = actionBeforeLoadScene
            };
            TransitionKit.instance.transitionWithDelegate(fader);
        }

        public static void ReplaceSceneImageMask(int sceneIndex, float secondsToWait = 1.0f)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }

            /*var mask = new ImageMaskTransition()
            {
                //maskTexture = PrefabControl.MakeMaskTexture(),
                backgroundColor = Color.black,
                nextScene = sceneIndex,
                duration = secondsToWait
            };
            TransitionKit.instance.transitionWithDelegate(mask);*/
        }



        public static void OpenCircle(int sceneIndex, float secondsToWait = 2f, float smoothness = 0.001f, GameObject targetObj = null, Color? c = null, Action actionBeforeLoadScene = null)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }

            var circle = new CircleOpeningTransition()
            {
                nextScene = sceneIndex,
                duration = secondsToWait,
                targetCenter = targetObj,
                smoothness = smoothness,
                backgroundColor = c ?? new Color(0, 0, 0, 1),
                actionCallbackBeforeLoadScene = actionBeforeLoadScene
            };
            TransitionKit.instance.transitionWithDelegate(circle);
        }

        //We have a Bug when this is used on Fire Devices.
        public static void OpenCircle(string sceneName, float secondsToWait = 2.0f, float smoothness = 0.01f, GameObject targetObj = null, Color? c = null , Action actionBeforeLoadScene = null)
        {
            if (Inst.UiVisible == false)
            {
                return;
            }

            var circle = new CircleOpeningTransition()
            {
                nextScene = -1,
                nextSceneName = sceneName,
                duration = secondsToWait,
                targetCenter = targetObj,
                smoothness = smoothness,
                backgroundColor = c ?? new Color(0, 0, 0, 1),
                actionCallbackBeforeLoadScene = actionBeforeLoadScene
            };
            TransitionKit.instance.transitionWithDelegate(circle);
        }

        private void OnEnable()
        {
            TransitionKit.onScreenObscured += onScreenObscured;
            TransitionKit.onTransitionComplete += onTransitionComplete;
        }

        private void OnDisable()
        {
            // as good citizens we ALWAYS remove event handlers that we added
            TransitionKit.onScreenObscured -= onScreenObscured;
            TransitionKit.onTransitionComplete -= onTransitionComplete;
        }

        private void IsOnScreenObscured()
        {
            if (Inst.UiVisible == false)
            {
                return;
            }
        }

        private void onScreenObscured()
        {
            _isUiVisible = false;
        }

        private void onTransitionComplete()
        {
            _isUiVisible = true;
        }
    }
}
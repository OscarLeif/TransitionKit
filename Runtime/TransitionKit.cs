using AtaGames.TransitionKit.runtime;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtaGames.TransitionKit
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TransitionKit : MonoBehaviour
    {
        public static LoadSceneMode LoadMode = LoadSceneMode.Single;

        public static bool IsWorking = false;
        public static bool keepTransitionKitInstance = true;

        public static Action OnTransitionStart;
        public static Action BeforeSceneLoad;
        public static Action AfterSceneLoad;
        public static Action OnTransitionCompleted;

        public static float DelayAfterLoad = 0.1f;//Need A Small Delay

        private static TransitionKit _instance;

        public static TransitionKit Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject(nameof(TransitionKit));
                    _instance = gameObject.AddComponent<TransitionKit>();
                }
                return _instance;
            }
        }

        //Suport for No DomainReload
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Setup()
        {
            _instance = null;
            IsWorking = false;
        }

        public bool useUnscaledDeltaTime = true;

        public Canvas canvas;
        public Material material;
        public RawImage RawImage;
        public bool UseCanvasScreenOverlay = true;

        private TransitionScene _transitionScene;

        public float deltaTime
        {
            get { return useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime; }
        }

        private void SetupBeforeInitialize()
        {
            material.shader = _transitionScene.shaderForTransition();
            if (UseCanvasScreenOverlay)
            {
                //Setup canvas screen-overlay
                this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                //setup Canvas screen-camera
                this.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
        }

        public IEnumerator TransitionWithDelegate(TransitionScene transitionKitDelegate)
        {
            _transitionScene = transitionKitDelegate;
            //Setup Here
            this.canvas = gameObject.getOrAddComponent<Canvas>();
            this.canvas.enabled = true;

            var Image = gameObject.getOrAddComponent<RawImage>();
            Image.material = material;
            Image.texture = material.mainTexture;

            material.mainTexture = _transitionScene.textureForDisplay();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 128;

            _transitionScene = transitionKitDelegate;
            SetupBeforeInitialize();
            gameObject.SetActive(true);

            OnTransitionStart?.Invoke();
            yield return _transitionScene.onScreenObscured(this);
            OnTransitionCompleted?.Invoke();

            TransitionCompleted();
        }

        //Material Update
        public IEnumerator tickProgressPropertyInMaterial(float duration, bool reverseDirection = false)
        {
            var start = reverseDirection ? 1f : 0f;
            var end = reverseDirection ? 0f : 1f;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += deltaTime;
                var step = Mathf.Lerp(start, end, Mathf.Pow(elapsed / duration, 2f));
                material.SetFloat(TransitionKitConstants._Progress, step);
                yield return null;
            }
        }

        public async Task tickProgressPropertyInMaterialTask(float duration, bool reverseDirection = false)
        {
            var start = reverseDirection ? 1f : 0f;
            var end = reverseDirection ? 0f : 1f;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += deltaTime;
                var step = Mathf.Lerp(start, end, Mathf.Pow(elapsed / duration, 2f));
                material.SetFloat(TransitionKitConstants._Progress, step);
                await Task.Yield();                
            }
        }

        public async void TransitionWithDelegateTask(TransitionScene transitionKitDelegate)
        {
            _transitionScene = transitionKitDelegate;
            //Setup Here
            this.canvas = gameObject.getOrAddComponent<Canvas>();
            this.canvas.enabled = true;

            var Image = gameObject.getOrAddComponent<RawImage>();
            Image.material = material;
            Image.texture = material.mainTexture;

            material.mainTexture = _transitionScene.textureForDisplay();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 128;

            _transitionScene = transitionKitDelegate;
            SetupBeforeInitialize();
            gameObject.SetActive(true);

            OnTransitionStart?.Invoke();
            await _transitionScene.onScreenObscuredTask(this);
            OnTransitionCompleted?.Invoke();
            TransitionCompleted();
        }

        private void TransitionCompleted()
        {
            TransitionKit.OnTransitionStart = null;
            TransitionKit.OnTransitionCompleted = null;
            TransitionKit.BeforeSceneLoad = null;
            TransitionKit.AfterSceneLoad = null;

            this.canvas.enabled = false;
        }

        #region Monobehaviour

        private void Awake()
        {
            DontDestroyOnLoad(this);
            material = new Material(Shader.Find(TransitionKitConstants.FadeShader));//Need a Shader to Initialize the Material.
        }

        #endregion
    }
}
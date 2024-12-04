using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(CanvasGroup))]
    //* SUMMARY
    //* This is a template class to script in scene transitions.
    public abstract class SceneTransition : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [Tooltip(
            "You can have scripts add their methods to this event, to be called before this transition starts."
        )]
        public UnityEvent beforeTransition;

        [Tooltip(
            "You can have scripts add their methods to this event, to be called after this transition ends."
        )]
        public UnityEvent afterTransition;

        [SerializeField]
        protected TMP_Text transitionDebugText;

        [SerializeField]
        protected TMP_Text loadingIndicator;

        [SerializeField]
        protected TMP_Text loadingText;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        [HideInInspector]
        public string transitionName;

        [HideInInspector]
        public Image background;

        [HideInInspector]
        public Animator animator;

        [HideInInspector]
        public Slider slider;

        [HideInInspector]
        public CanvasGroup canvasGroup;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            InitializeSceneTransition();
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        protected void InitializeSceneTransition()
        {
            transitionName = name;
            background = GetComponent<Image>();
            animator = GetComponent<Animator>();
            slider = GetComponent<Slider>();
            slider.fillRect = background.rectTransform;
            canvasGroup = GetComponent<CanvasGroup>();

            if (transitionDebugText == null && SimpleSceneTransitionManager.Instance.debugMode)
            {
                GameObject transitionDebugTextObject = new GameObject("Transition Debug Text");
                transitionDebugText = transitionDebugTextObject.AddComponent<TMP_Text>();
                transitionDebugText.text = "";
            }
        }

        protected virtual void UpdateDebugText()
        {
            transitionDebugText.gameObject.SetActive(
                SimpleSceneTransitionManager.Instance.debugMode
            );
            transitionDebugText.text =
                "Loading to \"" + SimpleSceneTransitionManager.Instance.targetSceneName + "\"...";
        }

        public abstract void PrimeTransition();
        public abstract void SwitchScene();
        public abstract void CleanUpTransition();
    }

    //* SUMMARY
    //* This manager handles scene switching as well as transitions for different scenes as it loads.
    public class SimpleSceneTransitionManager : MonoBehaviour, HasSignals
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleSceneTransitionManager instance;
        public static SimpleSceneTransitionManager Instance
        {
            get
            {
                if (instance == null)
                    CheckForExisitingSingleton(true);
                return instance;
            }
            private set { }
        }

        static void CheckForExisitingSingleton(bool fromGet)
        {
            if (instance != null)
                return;
            UnityEngine.Object[] existingSingleton = FindObjectsByType(
                typeof(SimpleSceneTransitionManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleSceneTransitionManager>();
                if (!logSingleton)
                    return;
                string logMessage;
                if (fromGet)
                    logMessage = "(Get)";
                else
                    logMessage = "(Awake)";
                Debug.Log(
                    logMessage
                        + " Preset singleton found; "
                        + existingSingleton[0].name
                        + " set as singleton instance."
                );
            }
            else
            {
                instance = new GameObject(
                    "Simple Scene Transition Manager"
                ).AddComponent<SimpleSceneTransitionManager>();
                Debug.LogWarning("New " + instance.gameObject.name + " created.");
            }
        }

        void Awake()
        {
            CheckForExisitingSingleton(false);
            transform.parent = null;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            if (instance == this)
            {
                NonSingletonAwake();
                return;
            }
            if (instance != null)
            {
                if (logSingleton)
                    Debug.Log(
                        name + " was overwritten by existing singleton: " + instance.gameObject.name
                    );
                Destroy(gameObject);
                return;
            }
            if (logSingleton)
                Debug.Log(name + " set as singleton instance.");
            instance = this;
            NonSingletonAwake();
        }

        void NonSingletonAwake() { }

        //! - - - - - - - - - - -

        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        public bool isTransitioning { get; private set; } = false;
        public List<SceneTransition> transitions = new List<SceneTransition>();

        [SerializeField]
        SceneTransition selectedTransition;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        public bool debugMode = false;

        [SerializeField]
        bool clearLogOnTransition = false;
        public string targetSceneName = "";
        public string selectedTransitionName = "Crossfade";

        [Space(10)]
        [Header("Timing")]
        public float delay = 0.0f;
        public float enteringDuration = 0.0f;
        public float exitingDuration = 0.0f;

        [Space(10)]
        [Header("Simple Game Event Manager")]
        [SerializedDictionary("Description", "Signal")]
        public SerializedDictionary<string, string> signals = new SerializedDictionary<
            string,
            string
        >
        {
            //? In-going
            //? Out-going
            { "When Current Scene is Unloaded.", "Unloading..." },
            { "When Incoming Scene is Loaded.", "Loading..." },
        };
        public SerializedDictionary<string, string> ownedSignals
        {
            get => signals;
            set => signals = value;
        }

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        string previousSceneName;
        float GUIWidth = 220;
        bool debugModeDrawn = false;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            foreach (Transform transition in transform.Find("Transitions").transform)
            {
                if (
                    transition.GetComponent<SceneTransition>() != null
                    && transition.gameObject.activeInHierarchy
                )
                    transitions.Add(transition.GetComponent<SceneTransition>());
            }
        }

        void Update()
        {
            if (debugMode)
            {
                ShowDebugButtons();
                debugModeDrawn = true;
            }
            else
            {
                CleanUpDebugButtons();
                debugModeDrawn = false;
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        void ShowDebugButtons()
        {
            if (debugModeDrawn)
                return;

            SimpleDebuggingManager.Instance.SetWidth(GUIWidth);

            UnityEvent loadSceneWithTransition = new UnityEvent();
            loadSceneWithTransition.AddListener(LoadSceneWithTransition);
            SimpleDebuggingManager.Instance.AddDebugOption(
                "LoadSceneWithTransition",
                "Load Scene With Transition",
                "Loads the Target Scene with the selected Transition",
                loadSceneWithTransition
            );

            UnityEvent loadSceneWithoutTransition = new UnityEvent();
            loadSceneWithoutTransition.AddListener(LoadSceneWithoutTransition);
            SimpleDebuggingManager.Instance.AddDebugOption(
                "LoadSceneWithoutTransition",
                "Load Scene Without Transition",
                "Loads the Target Scene",
                loadSceneWithoutTransition
            );

            UnityEvent addToCurrentScene = new UnityEvent();
            addToCurrentScene.AddListener(LoadSceneWithoutTransition);
            SimpleDebuggingManager.Instance.AddDebugOption(
                "AddToCurrentScene",
                "Add To Current Scene",
                "Adds the Target Scene onto the Current Scene",
                addToCurrentScene
            );

            UnityEvent returnToPreviousScene = new UnityEvent();
            returnToPreviousScene.AddListener(ReturnToPreviousScene);
            SimpleDebuggingManager.Instance.AddDebugOption(
                "ReturnToPreviousScene",
                "Return To Previous Scene",
                "Return tp the previously Loaded Scene",
                returnToPreviousScene
            );
        }

        void CleanUpDebugButtons()
        {
            if (!debugModeDrawn)
                return;
            SimpleDebuggingManager.Instance.RemoveDebugOption("LoadSceneWithTransition");
            SimpleDebuggingManager.Instance.RemoveDebugOption("LoadSceneWithoutTransition");
            SimpleDebuggingManager.Instance.RemoveDebugOption("AddToCurrentScene");
            SimpleDebuggingManager.Instance.RemoveDebugOption("ReturnToPreviousScene");
        }

        public void LoadSceneWithTransition()
        {
            if (targetSceneName == "")
            {
                Debug.LogError(name + " has no Target Scene");
                return;
            }
            if (selectedTransitionName == "")
            {
                Debug.LogWarning(
                    name + " has no Selected Transition; switching with no transition"
                );
                LoadSceneWithoutTransition();
                return;
            }
            if (isTransitioning)
            {
                Debug.LogWarning(name + " is already transitioning to " + targetSceneName);
                return;
            }

            previousSceneName = SceneManager.GetActiveScene().name;
            selectedTransition = FindTransition(selectedTransitionName);
            Debug.Log(
                "Transitioning to \""
                    + targetSceneName
                    + "\" with "
                    + selectedTransitionName
                    + " transition..."
            );
            Invoke("TriggerTransition", delay);
        }

        [ContextMenu("Trigger Transition")]
        public void TriggerTransition()
        {
            selectedTransition.PrimeTransition();
            ActivateTriggers(false);
            TransitionAnimationTrigger(true);
        }

        public void LoadSceneWithTransition(string targetScene)
        {
            targetSceneName = targetScene;
            LoadSceneWithTransition();
        }

        public void LoadSceneWithTransition(string targetScene, string transitionName)
        {
            targetSceneName = targetScene;
            selectedTransitionName = transitionName;
            LoadSceneWithTransition();
        }

        public void LoadSceneWithoutTransition()
        {
            if (isTransitioning)
                return;
            previousSceneName = SceneManager.GetActiveScene().name;
            ActivateTriggers(false);
            LoadToTargetScene();
            ActivateTriggers(true);
        }

        public void LoadToTargetScene()
        {
            Debug.Log("Loading to \"" + targetSceneName + "\"...");
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
            if (selectedTransitionName != "" && selectedTransition != null)
                EndTransition();
        }

        public void AddToCurrentScene()
        {
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive);
        }

        public void AddToCurrentScene(string transitionName)
        {
            targetSceneName = transitionName;
            AddToCurrentScene();
        }

        public void ReturnToPreviousScene()
        {
            if (isTransitioning)
                return;
            // targetSceneName = previousSceneName;
            LoadSceneWithTransition();
        }

        public void EndTransition()
        {
            ActivateTriggers(true);
            TransitionAnimationTrigger(false);
            // if (clearLogOnTransition)
                // ClearDebugLog.DoThis.Please();
            // targetSceneName = "";
        }

        SceneTransition FindTransition(string transtionName)
        {
            foreach (SceneTransition transition in transitions)
                Debug.Log(transition.name);
            if (transitions.Find(transition => transition.transitionName == transtionName) == null)
            {
                Debug.LogError("A Scene Transition called \"" + transtionName + "\" was not found");
                return null;
            }
            return transitions.Find(transition => transition.transitionName == transtionName);
        }

        void ActivateTriggers(bool onEnter)
        {
            if (onEnter)
                SimpleGameEventManager.Instance.SendSignal(
                    targetSceneName + signals["When Incoming Scene is Loaded."]
                );
            else
                SimpleGameEventManager.Instance.SendSignal(
                    targetSceneName + signals["When Current Scene is Unloaded."]
                );
        }

        void TransitionAnimationTrigger(bool flag)
        {
            isTransitioning = flag;
            if (selectedTransitionName == "")
                return;
            selectedTransition.animator = selectedTransition.GetComponent<Animator>();
            selectedTransition?.animator?.SetBool("isTransitioning", isTransitioning);
        }
    }
}

using System;
using System.Collections.Generic;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    [Serializable]
    public class DebugOption
    {
        public string name = "New Debug Option";
        public string text = "New Debug Option";
        public bool enabled = true;
        public Texture texture;
        public string description = "";
        public UnityEvent triggers = new UnityEvent();
    }

    public class SimpleDebuggingManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleDebuggingManager instance;
        public static SimpleDebuggingManager Instance
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
                typeof(SimpleDebuggingManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleDebuggingManager>();
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
                    "Simple Debugging Manager"
                ).AddComponent<SimpleDebuggingManager>();
                Debug.LogWarning("New " + instance.name + " created.");
            }
        }

        void Awake()
        {
            CheckForExisitingSingleton(false);
            transform.parent = null;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            if (instance == this)
                return;
            if (instance != null)
            {
                if (logSingleton)
                    Debug.Log(
                        name
                            + " was overwritten by by existing singleton: "
                            + instance.gameObject.name
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
        public List<DebugOption> debugOptions = new List<DebugOption>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        bool debugMode = false;

        [Space(10)]
        [Header("Appearance")]
        [SerializeField]
        float originalLeftOffset = 20;

        [SerializeField]
        float originalTopOffset = 20;

        [SerializeField]
        float originalWidth = 150;

        [SerializeField]
        float originalHeight = 30;

        [SerializeField]
        float originalGap = 10;

        [SerializeField]
        float optionsPerColumn = 5;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        float leftOffset = 20;
        float topOffset = 20;
        float width = 150;
        float height = 30;
        float gap = 10;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            Reset();
        }

        void OnGUI()
        {
            if (!debugMode)
                return;

            int currentRow = 0;
            int currentColumn = 0;
            foreach (DebugOption debugOption in debugOptions)
            {
                if (debugOption.enabled)
                    if (
                        GUI.Button(
                            new Rect(
                                leftOffset + (currentColumn * width) + (gap * currentColumn),
                                topOffset + (currentRow * height) + (gap * currentRow),
                                width,
                                height
                            ),
                            new GUIContent(
                                debugOption.text,
                                debugOption.texture,
                                debugOption.description
                            )
                        )
                    )
                    {
                        debugOption.triggers?.Invoke();
                    }

                currentRow++;
                if (currentRow % optionsPerColumn == 0)
                {
                    currentRow = 0;
                    currentColumn++;
                }
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        [ContextMenu("Reset Debug Options")]
        public void Reset()
        {
            debugOptions.Clear();
            leftOffset = originalLeftOffset;
            topOffset = originalTopOffset;
            width = originalWidth;
            height = originalHeight;
            gap = originalGap;
        }

        [ContextMenu("Test Layout")]
        public void TestLayout()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("You can only preview the layout while the game is running");
                return;
            }

            debugMode = true;
            UnityEvent sampleEvent = new UnityEvent();
            sampleEvent.AddListener(FunctionWithNoParameters);
            Reset();

            for (int row = 0; row < optionsPerColumn; row++)
            {
                for (int column = 0; column < optionsPerColumn; column++)
                {
                    AddDebugOption(
                        "Sample #" + debugOptions.Count,
                        "Sample (" + row + ", " + column + ")",
                        (row + column) % 2 == 0,
                        "This is Sample #" + debugOptions.Count,
                        sampleEvent
                    );
                }
            }
        }

        public void AddDebugOption(string name, string text, UnityEvent triggers)
        {
            if (CheckIfDebugOptionExists(name))
            {
                Debug.Log("A Debug Option named \"" + name + "\" already exists");
                return;
            }
            DebugOption newDebugOption = new DebugOption();
            newDebugOption.name = name;
            newDebugOption.text = text;
            newDebugOption.enabled = true;
            newDebugOption.description = "";
            newDebugOption.triggers = triggers;
            debugOptions.Add(newDebugOption);
        }

        public void AddDebugOption(string name, string text, bool enabled, UnityEvent triggers)
        {
            if (CheckIfDebugOptionExists(name))
            {
                Debug.Log("A Debug Option named \"" + name + "\" already exists");
                return;
            }
            DebugOption newDebugOption = new DebugOption();
            newDebugOption.name = name;
            newDebugOption.text = text;
            newDebugOption.enabled = enabled;
            newDebugOption.description = "";
            newDebugOption.triggers = triggers;
            debugOptions.Add(newDebugOption);
        }

        public void AddDebugOption(
            string name,
            string text,
            string description,
            UnityEvent triggers
        )
        {
            if (CheckIfDebugOptionExists(name))
            {
                Debug.Log("A Debug Option named \"" + name + "\" already exists");
                return;
            }
            DebugOption newDebugOption = new DebugOption();
            newDebugOption.name = name;
            newDebugOption.text = text;
            newDebugOption.enabled = true;
            newDebugOption.description = description;
            newDebugOption.triggers = triggers;
            debugOptions.Add(newDebugOption);
        }

        public void AddDebugOption(
            string name,
            string text,
            bool enabled,
            string description,
            UnityEvent triggers
        )
        {
            if (CheckIfDebugOptionExists(name))
            {
                Debug.Log("A Debug Option named \"" + name + "\" already exists");
                return;
            }
            DebugOption newDebugOption = new DebugOption();
            newDebugOption.name = name;
            newDebugOption.text = text;
            newDebugOption.enabled = enabled;
            newDebugOption.description = description;
            newDebugOption.triggers = triggers;
            debugOptions.Add(newDebugOption);
        }

        void FunctionWithNoParameters()
        {
            Debug.Log("I'm a Sample!");
        }

        public DebugOption GetDebugOption(string name)
        {
            return debugOptions.Find(debugOption => debugOption.name == name);
        }

        public void RemoveDebugOption(string name)
        {
            if (!CheckIfDebugOptionExists(name))
            {
                Debug.Log("There is no Debug Option named \"" + name + "\" to remove");
                return;
            }
            debugOptions.Remove(debugOptions.Find(debugOption => debugOption.name == name));
        }

        bool CheckIfDebugOptionExists(string name)
        {
            return GetDebugOption(name) != null;
        }

        public void SetLeftOffset(float newLeftOffset)
        {
            if (leftOffset < newLeftOffset)
                leftOffset = newLeftOffset;
        }

        public void SetTopOffset(float newTopOffset)
        {
            if (topOffset < newTopOffset)
                topOffset = newTopOffset;
        }

        public void SetWidth(float newWidth)
        {
            if (width < newWidth)
                width = newWidth;
        }

        public void SetHeight(float newHeight)
        {
            if (height < newHeight)
                height = newHeight;
        }

        public void SetGap(float newGap)
        {
            if (gap < newGap)
                gap = newGap;
        }

        public void ToggleSceneInteraction() { }
    }
}

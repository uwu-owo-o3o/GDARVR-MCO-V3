using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace LilLycanLord_Official
{
    public interface HasSignals
    {
        [Header("Simple Game Event Manager")]
        [SerializedDictionary("Description", "Signal")]
        public SerializedDictionary<string, string> ownedSignals { get; set; }
    }

    public class SimpleGameEventManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleGameEventManager instance;
        public static SimpleGameEventManager Instance
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
                typeof(SimpleGameEventManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleGameEventManager>();
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
                        + instance.name
                        + " set as singleton instance."
                );
            }
            else
            {
                instance = new GameObject(
                    "Simple Game Event Manager"
                ).AddComponent<SimpleGameEventManager>();
                Debug.LogWarning("New " + instance.gameObject.name + " created.");
            }
        }

        void Awake()
        {
            CheckForExisitingSingleton(false);
            transform.parent = null;
            // if (transform.parent == null)
            //     DontDestroyOnLoad(gameObject);
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
        [Tooltip(
            "Tracked Events, and Signals they listen to. NOTE: UnityEvents' displays' may bug out, not showing the action, but it is there."
        )]
        [SerializedDictionary("Signal", "Events")]
        public SerializedDictionary<string, UnityEvent> events =
            new SerializedDictionary<string, UnityEvent>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [Tooltip(
            "Manually simulate a Signal being thrown by using \"Manual Signal\" in the context menu. Signals are often laid out as \"[INTENT/EVENT NAME] Sender Name\""
        )]
        [SerializeField]
        string manualSignal = "";

        [SerializeField]
        bool debugMode = false;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start() { }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        //* SUMMARY
        //* Called to subscribe a script's method as a UnityAction to a UnityEvent
        public void AddAction(string signalName, UnityAction action, GameObject gameObject = null)
        {
            //? If a UnityEvent is already waiting for an existing Signal,
            //? simply add the UnityAction as a listener to that UnityEvent.
            signalName = "[" + signalName + "]";
            if (gameObject != null)
                signalName = "[" + gameObject.name + "] -> " + signalName;

            if (events.ContainsKey(signalName))
            {
                events[signalName].AddListener(action);
                if (debugMode)
                    Debug.Log(
                        "\"" + action.Method.Name + "\" is now waiting for \"" + signalName + "\""
                    );
            }
            //? else, add a new Signal with a new UnityEvent with the UnityAction
            //? as its listener.
            else
            {
                events.Add(signalName, new UnityEvent());
                events[signalName].AddListener(action);
                if (debugMode)
                    Debug.Log(
                        "\""
                            + action.Method.Name
                            + "\" is now waiting for a new Signal called \""
                            + signalName
                            + "\""
                    );
            }
        }

        //* SUMMARY
        //* Called to unsubscribe a script's method from an existing UnityEvent
        public void RemoveAction(
            string signalName,
            UnityAction action,
            GameObject gameObject = null
        )
        {
            signalName = "[" + signalName + "]";
            if (gameObject != null)
                signalName = "[" + gameObject.name + "] -> " + signalName;

            if (events.ContainsKey(signalName))
            {
                events[signalName].RemoveListener(action);
                if (debugMode)
                    Debug.Log("\"" + action + "\" has stopped waiting for \"" + signalName + "\"");
            }
        }

        [ContextMenu("Manual Signal")]
        //* SUMMARY
        //* Called via the context menu, this is to simulate a Signal being sent somewhere during runtime.
        void ManualSignal()
        {
            SendSignal(manualSignal);
        }

        //* SUMMARY
        //* Called to trigger a UnityEvent, or a set of UnityActions/methods via string.
        public void SendSignal(string signalName, GameObject signalSender = null)
        {
            if (signalName == "")
            {
                if (signalSender != null)
                    Debug.LogWarning("Blank Signal sent by" + signalSender.name + ".");
                else
                    Debug.LogWarning("Blank Signal sent.");
                return;
            }
            signalName = "[" + signalName + "]";
            if (signalSender != null)
                signalName = "[" + signalSender.name + "] -> " + signalName;
            if (debugMode)
                Debug.Log("Signal Sent: \"" + signalName + "\"");
            if (events.ContainsKey(signalName))
                events[signalName]?.Invoke();
            else if (debugMode)
                Debug.LogWarning("There is no Signal called \"" + signalName + "\"");
        }
    }
}

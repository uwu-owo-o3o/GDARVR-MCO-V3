using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace LilLycanLord_Official
{
    public class TaskManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static TaskManager instance;
        public static TaskManager Instance
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
                typeof(TaskManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<TaskManager>();
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
                instance = new GameObject("Task Manager").AddComponent<TaskManager>();
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
        [SerializedDictionary("Task", "Countdown")]
        public SerializedDictionary<GameObject, int> tasks =
            new SerializedDictionary<GameObject, int>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        int taskChance = 3;

        [SerializeField]
        GameObject taskPopUpPrefab;

        [SerializeField]
        GameObject taskPrefab;

        [SerializeField]
        Canvas generalCanvas;

        [SerializeField]
        Canvas activeTaskList;

        [SerializeField]
        Canvas expiredTaskList;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        GameObject activeListContent;
        GameObject expiredListContent;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            activeListContent = activeTaskList
                .transform.Find("Scroll View")
                .Find("Viewport")
                .Find("Content")
                .gameObject;
            expiredListContent = expiredTaskList
                .transform.Find("Scroll View")
                .Find("Viewport")
                .Find("Content")
                .gameObject;
        }

        void Update()
        {
            // Debug.Log(SceneManager.GetActiveScene().name);
            if (activeListContent != null)
            {
                activeListContent.SetActive(SceneManager.GetActiveScene().name == "Tasks");
                //: NOTE: Enable this if-block on build/implementation.
                if (SceneManager.GetActiveScene().name == "Tasks")
                {
                    generalCanvas.enabled = false;
                    expiredTaskList.enabled = false;
                }
            }

            activeTaskList.enabled = expiredListContent.transform.childCount == 0;
            expiredTaskList.enabled = expiredListContent.transform.childCount > 0;
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        [ContextMenu("Get Task")]
        public void GetTask()
        {
            if (Random.Range(1, taskChance) == 1)
            {
                Debug.Log("Task Get!");
                GameObject.Instantiate(taskPopUpPrefab, generalCanvas.transform);
            }
            else
                Debug.Log("Failed to get task");
        }

        public void AddTask(int taskID)
        {
            GameObject newTask = GameObject.Instantiate(taskPrefab, activeListContent.transform);
            newTask.GetComponent<TaskBehaviour>().SetTask(taskID);
            tasks.Add(newTask, 3);
        }

        public void ResolveTask(TaskBehaviour task)
        {
            tasks.Remove(task.gameObject);
        }

        public void CheckTasks()
        {
            List<GameObject> keys = new List<GameObject>(tasks.Keys);
            foreach (GameObject task in keys)
            {
                tasks[task]--;
                if (tasks[task] == 0)
                    OnTaskExpired(task);
            }
        }

        void OnTaskExpired(GameObject task)
        {
            SimpleAudioManager.Instance.Play("Expired Task", gameObject);
            task.GetComponent<RectTransform>().localScale = Vector3.zero;
            task.GetComponent<RectTransform>().SetParent(expiredListContent.transform);
            task.GetComponent<TaskBehaviour>().Grow();
        }
    }
}

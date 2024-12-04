using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    public class TaskProgression : MonoBehaviour
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
        [SerializeField]
        RawImage checkingTasksPrompt;

        [SerializeField]
        float checkDelay = 3.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start() { }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void CheckTasks()
        {
            Debug.Log("Checking tasks...");
            checkingTasksPrompt.enabled = true;
            Invoke("TriggerCheck", checkDelay);
        }

        void TriggerCheck()
        {
            checkingTasksPrompt.enabled = false;
            TaskManager.Instance.CheckTasks();
        }
    }
}

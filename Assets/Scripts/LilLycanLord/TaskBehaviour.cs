using System.Collections.Generic;
using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(RawImage))]
    public class TaskBehaviour : TaskPopUp
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
        // [Space(10)]
        // [Header("Fields")]

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Start() { }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void SetTask(int taskID)
        {
            rawImage = GetComponent<RawImage>();
            rawImage.texture = taskTextures[taskID - 1];
        }

        public void Resolve()
        {
            TaskManager.Instance.ResolveTask(this);
            Shrink();
        }
    }
}

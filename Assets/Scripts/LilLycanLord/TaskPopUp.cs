using System.Collections.Generic;
using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RawImage))]
    public class TaskPopUp : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        protected RectTransform rectTransform;
        protected RawImage rawImage;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        public int taskID = -1;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        protected float growDuration = 1.0f;

        [SerializeField]
        protected float shrinkDuration = 1.0f;

        [SerializeField]
        protected List<Texture> taskTextures = new List<Texture>();

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.zero;

            rawImage = GetComponent<RawImage>();
            taskID = Random.Range(1, taskTextures.Count + 1);
            rawImage.texture = taskTextures[taskID - 1];

            Grow();
        }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        [ContextMenu("Grow")]
        public void Grow()
        {
            rectTransform.LeanScale(Vector3.one, growDuration).setEase(LeanTweenType.easeOutBounce);
        }

        [ContextMenu("Shrink")]
        public void Shrink()
        {
            rectTransform
                .LeanScale(Vector3.zero, shrinkDuration)
                .setEase(LeanTweenType.easeInBounce);

            Destroy(gameObject, shrinkDuration);
        }

        public void Accept()
        {
            TaskManager.Instance.AddTask(taskID);
            Shrink();
        }

        public void Discard()
        {
            Shrink();
        }
    }
}

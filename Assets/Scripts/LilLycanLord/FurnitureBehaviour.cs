using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class FurnitureBehaviour : MonoBehaviour, ISpreadable, ISwipeable, IDraggable
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
        void Awake() { }

        void Start() { }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void OnSpread(SpreadEventArgs spreadArgs)
        {
            transform.localScale *= spreadArgs.DistanceDelta;
        }

        public void OnSwipe(SwipeEventArgs swipeArgs)
        {
            transform.Rotate(transform.up, 45.0f);
        }

        public void OnDrag(DragEventArgs dragArgs)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward;
        }
    }
}

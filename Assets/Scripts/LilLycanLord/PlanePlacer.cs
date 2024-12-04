using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;

namespace LilLycanLord_Official
{
    public class PlanePlacer : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        ARPlaneManager planeManager;

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
        GameObject anchoredPrefab;

        [SerializeField]
        float forwardOffset = 2.0f;
        [SerializeField]
        float scaleMultiplier = 1.0f;


        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        GameObject planePool;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            planeManager = GetComponent<ARPlaneManager>();
            planePool = new GameObject("Planes");
            planePool.transform.parent = gameObject.transform;
        }

        void Update()
        {
            Vector3 touchPosition = CheckTouch();
            if (touchPosition != Vector3.zero)
                PlaceAnchor(touchPosition);
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        Vector3 CheckTouch()
        {
            if (Input.touchCount <= 0)
                return Vector3.zero;
            
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return Vector3.zero;

            Vector3 targetPosition = Camera
                .main.ScreenPointToRay(Input.GetTouch(0).position)
                .GetPoint(forwardOffset);
            return targetPosition;
        }

        void PlaceAnchor(Vector3 position)
        {
            GameObject newAnchor = GameObject.Instantiate(
                anchoredPrefab,
                position,
                Quaternion.identity,
                planePool.transform
            );
            newAnchor.transform.localScale *= scaleMultiplier;
            newAnchor.AddComponent<ARAnchor>();
        }
    }
}

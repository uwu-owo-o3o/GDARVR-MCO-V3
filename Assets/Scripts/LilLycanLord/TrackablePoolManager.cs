using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;

namespace LilLycanLord_Official
{
    public enum InputMode
    {
        PC,
        Mobile,
        Individual
    }

    [RequireComponent(typeof(ARRaycastManager))]
    public class TrackablePoolManager : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        ARRaycastManager raycastManager;
        ARPlaneManager planeManager;
        ARPointCloudManager pointCloudManager;
        ARAnchorManager anchorManager;
        ARTrackedImageManager trackedImageManager;
        AREnvironmentProbeManager environmentProbeManager;
        ARFaceManager faceManager;
        ARTrackedObjectManager trackedObjectManager;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        public InputMode globalInputMode = InputMode.PC;

        [SerializeField]
        float updatesPerSecond = 1.0f;

        [SerializeField]
        string destroyTrackableType;

        [SerializeField]
        bool planeVisibility = false;

        [SerializedDictionary("Trackable", "Maximum Count")]
        public SerializedDictionary<string, int> maximumPoolSizes = new SerializedDictionary<
            string,
            int
        >
        {
            { "ARPlane", 20 },
            { "ARPointCloud", 20 },
            { "ARAnchor", 20 },
            { "ARTrackedImage", 20 },
            { "AREnvironmentProbe", 20 },
            { "ARFace", 20 },
            { "ARTrackedObject", 20 },
        };
        public List<GameObject> trackablePools = new List<GameObject>();

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        float updateTimer = 0.0f;
        GameObject poolPool;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            poolPool = new GameObject("Trackable Pools");
            poolPool.transform.parent = transform;
            foreach (string trackableType in maximumPoolSizes.Keys)
            {
                GameObject newPool = new GameObject(trackableType + " Pool");
                newPool.transform.parent = poolPool.transform;
                trackablePools.Add(newPool);
            }

            GetManagerComponents();
        }

        void Update()
        {
            if (updateTimer >= (1.0f / updatesPerSecond))
            {
                UpdatePools();
                updateTimer = 0.0f;
            }
            else
                updateTimer += Time.deltaTime;
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        void UpdatePools()
        {
            foreach (string trackableType in maximumPoolSizes.Keys)
            {
                List<UnityEngine.Object> trackables = new List<UnityEngine.Object>();
                List<UnityEngine.Object> activeTrackables = new List<UnityEngine.Object>();

                switch (trackableType)
                {
                    case "ARPlane":
                        trackables = FindObjectsByType(
                                typeof(ARPlane),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        foreach (UnityEngine.Object trackable in trackables)
                            trackable.GetComponent<ARPlaneMeshVisualizer>().enabled =
                                planeVisibility;
                        break;
                    case "ARPointCloud":
                        trackables = FindObjectsByType(
                                typeof(ARPointCloud),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    case "ARAnchor":
                        trackables = FindObjectsByType(
                                typeof(ARAnchor),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    case "ARTrackedImage":
                        trackables = FindObjectsByType(
                                typeof(ARTrackedImage),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    case "AREnvironmentProbe":
                        trackables = FindObjectsByType(
                                typeof(AREnvironmentProbe),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    case "ARFace":
                        trackables = FindObjectsByType(
                                typeof(ARFace),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    case "ARTrackedObject":
                        trackables = FindObjectsByType(
                                typeof(ARTrackedObject),
                                FindObjectsSortMode.InstanceID
                            )
                            .ToList();
                        break;
                    default:
                        Debug.LogError("There is no AR Trackable called \"" + trackableType + "\"");
                        continue;
                }

                if (trackables.Count > 0)
                    foreach (UnityEngine.Object trackable in trackables)
                        trackable.GetComponent<Transform>().parent = poolPool.transform.Find(
                            trackableType + " Pool"
                        );

                foreach (UnityEngine.Object trackable in trackables)
                    if (trackable.GetComponent<Transform>().gameObject.activeSelf)
                        activeTrackables.Add(trackable);

                while (activeTrackables.Count > maximumPoolSizes[trackableType])
                {
                    if (activeTrackables[0] != null)
                        activeTrackables[0].GetComponent<Transform>().gameObject.SetActive(false);
                    activeTrackables.RemoveAt(0);
                }
            }
        }

        [ContextMenu("Destroy Trackables")]
        void DestroyTrackables()
        {
            DestroyTrackables(destroyTrackableType);
        }

        public int DestroyTrackables(string type)
        {
            int count = 0;
            Transform trackablePool = poolPool.transform.Find(type + " Pool");
            if (trackablePool == null)
                Debug.LogError("There is no AR Trackable called \"" + type + "\"");
            else
                foreach (Transform trackable in trackablePool)
                    trackable.gameObject.SetActive(false);
            Debug.LogWarning(name + " destroyed " + count + " " + type + "s");
            return count;
        }

        public Ray CheckTouch()
        {
            if (Input.touchCount <= 0)
                return new Ray();

            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return new Ray();

            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            return touchRay;
        }

        public ARTrackable RaycastTillTrackable(
            Ray inputRay,
            UnityEngine.XR.ARSubsystems.TrackableType trackableType
        )
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (raycastManager.Raycast(inputRay, hits, trackableType))
                return hits[0].trackable;
            return null;
        }

        void GetManagerComponents()
        {
            if (TryGetComponent<ARRaycastManager>(out ARRaycastManager raycastManagerComponent))
                raycastManager = raycastManagerComponent;
            if (TryGetComponent<ARPlaneManager>(out ARPlaneManager planeManagerComponent))
                planeManager = planeManagerComponent;
            if (
                TryGetComponent<ARPointCloudManager>(
                    out ARPointCloudManager pointCloudManagerComponent
                )
            )
                pointCloudManager = pointCloudManagerComponent;
            if (TryGetComponent<ARAnchorManager>(out ARAnchorManager anchorManagerComponent))
                anchorManager = anchorManagerComponent;
            if (
                TryGetComponent<ARTrackedImageManager>(
                    out ARTrackedImageManager trackedImageManagerComponent
                )
            )
                trackedImageManager = trackedImageManagerComponent;
            if (
                TryGetComponent<AREnvironmentProbeManager>(
                    out AREnvironmentProbeManager environmentProbeManagerComponent
                )
            )
                environmentProbeManager = environmentProbeManagerComponent;
            if (TryGetComponent<ARFaceManager>(out ARFaceManager faceManagerComponent))
                faceManager = faceManagerComponent;
            if (
                TryGetComponent<ARTrackedObjectManager>(
                    out ARTrackedObjectManager trackedObjectManagerComponent
                )
            )
                trackedObjectManager = trackedObjectManagerComponent;
        }
    }
}

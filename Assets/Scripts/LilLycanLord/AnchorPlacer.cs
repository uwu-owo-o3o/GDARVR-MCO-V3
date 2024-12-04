using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace LilLycanLord_Official
{
    public class AnchorPlacer : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        ARAnchorManager anchorManager;
        TrackablePoolManager trackablePoolManager;

        [SerializeField]
        TMP_Text offsetLabel;

        [SerializeField]
        TMP_Text scaleLabel;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [SerializedDictionary("Prefab Name", "Count")]
        public SerializedDictionary<string, int> prefabCounts =
            new SerializedDictionary<string, int>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        InputMode inputMode;
        public UnityEngine.XR.ARSubsystems.TrackableType validTrackableToPlaceAnchor;

        [SerializeField]
        GameObject prefabToAnchor;

        [SerializedDictionary("Prefab Name", "Maximum Count")]
        public SerializedDictionary<GameObject, int> prefabs =
            new SerializedDictionary<GameObject, int>();

        [SerializeField]
        float maximumForwardOffset = 20.0f;

        public float forwardOffsetPercentage = 1.0f;

        [SerializeField]
        [Range(0.1f, 5.0f)]
        float scaleMultiplier = 1.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        int anchorsPlaced = 0;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            anchorManager = GetComponent<ARAnchorManager>();
            trackablePoolManager = GetComponent<TrackablePoolManager>();
            if (inputMode == InputMode.Individual)
            {
                Debug.LogWarning(
                    "The Individual input mode is reserved for the TrackablePoolManager; Do not use."
                );
                inputMode = InputMode.PC;
            }
            if (trackablePoolManager.globalInputMode != InputMode.Individual)
                inputMode = trackablePoolManager.globalInputMode;

            foreach (GameObject prefab in prefabs.Keys)
                prefabCounts.Add(prefab.name, 0);
        }

        void Update()
        {
            Ray inputRay = new Ray();
            OffsetLabelUpdate();
            ScaleLabelUpdate();
            switch (inputMode)
            {
                case InputMode.PC:
                    if (Input.GetMouseButtonDown(0))
                        inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    break;
                case InputMode.Mobile:
                    inputRay = trackablePoolManager.CheckTouch();
                    break;
                default:
                    break;
            }

            // if (inputRay.origin != Vector3.zero)
            //     if (!DeleteAnchor(inputRay))
            //         if (
            //             validTrackableToPlaceAnchor
            //             == UnityEngine.XR.ARSubsystems.TrackableType.None
            //         )
            //             PlaceAnchor(
            //                 inputRay.GetPoint(maximumForwardOffset * forwardOffsetPercentage)
            //             );
            //         else
            //             PlaceAnchor(
            //                 trackablePoolManager
            //                     .RaycastTillTrackable(inputRay, validTrackableToPlaceAnchor)
            //                     .pose.position
            //             );
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝

        bool DeleteAnchor(Ray position)
        {
            if (Physics.Raycast(position, out RaycastHit hits))
            {
                Debug.Log("Destroying...");
                if (hits.collider.gameObject.GetComponentInParent<ARAnchor>() != null)
                {
                    Debug.Log(hits.collider.gameObject.name + " destroyed!");
                    prefabCounts[hits.collider.gameObject.name]--;
                    Destroy(hits.collider.gameObject);
                    offsetLabel.color = Color.red;
                    return true;
                }
            }
            return false;
        }

        void PlaceAnchor(Vector3 position)
        {
            if (prefabCounts[prefabToAnchor.name] >= prefabs[prefabToAnchor])
            {
                Debug.LogWarning(
                    "Maximum instances of \""
                        + prefabToAnchor.name
                        + "\" has been reached: "
                        + prefabCounts[prefabToAnchor.name]
                );
                return;
            }
            if (validTrackableToPlaceAnchor == UnityEngine.XR.ARSubsystems.TrackableType.None)
                Debug.Log(prefabToAnchor.name + " was placed at " + position);
            GameObject newAnchor = new GameObject(
                "Placed Anchor #" + anchorsPlaced,
                typeof(ARAnchor)
            );
            anchorsPlaced++;
            prefabCounts[prefabToAnchor.name]++;
            newAnchor.transform.position = position;
            GameObject prefabContent = GameObject.Instantiate(prefabToAnchor, newAnchor.transform);
            prefabContent.transform.localScale *= scaleMultiplier;
            prefabContent.name = prefabToAnchor.name;

            offsetLabel.color = Color.green;
        }

        public void SelectPrefab(int index)
        {
            if (index < 0 || index > prefabs.Count)
            {
                Debug.LogError("Selected Prefab Index Out of Bounds");
                return;
            }

            prefabToAnchor = prefabs.ElementAt(index).Key;
        }

        public void DeleteAllAnchors() { }

        public void TogglePointCloud(bool enabled)
        {
            foreach (
                ARPointCloudParticleVisualizer pointCloud in gameObject.GetComponentsInChildren<ARPointCloudParticleVisualizer>()
            )
                pointCloud.enabled = enabled;
        }

        void OffsetLabelUpdate()
        {
            if (offsetLabel == null)
                return;
            offsetLabel.text = (
                Mathf.Clamp(
                    maximumForwardOffset * forwardOffsetPercentage,
                    0.1f,
                    maximumForwardOffset
                )
            ).ToString("F2");
        }

        void ScaleLabelUpdate()
        {
            if (scaleLabel == null)
                return;
            scaleLabel.text = scaleMultiplier.ToString("F2");
        }

        public void OffsetSlider(float sliderValue)
        {
            forwardOffsetPercentage = sliderValue;
        }

        public void ScaleSlider(float sliderValue)
        {
            scaleMultiplier = sliderValue;
        }
    }
}

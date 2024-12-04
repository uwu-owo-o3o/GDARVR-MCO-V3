using System.Collections.Generic;
using LilLycanLord_Official;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace LilLycanLord_Official
{
    public class FurniturePlacer : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        ARAnchorManager anchorManager;
        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;

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
        GameObject furnitureToPlace;

        [SerializeField]
        List<GameObject> furnitures = new List<GameObject>();

        [SerializeField]
        float maximumScale = 3.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float scalePercentage = 1.0f;

        [SerializeField]
        float maximumHeightOffset = 2.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float heightOffsetPercentage = 1.0f;

        [SerializeField]
        Slider scaleSlider;

        [SerializeField]
        TMP_Text scaleLabel;

        [SerializeField]
        Slider heightSlider;

        [SerializeField]
        TMP_Text heightLabel;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        GameObject furniturePool;
        float labelReset = 0.0f;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            anchorManager = GetComponent<ARAnchorManager>();
            planeManager = GetComponent<ARPlaneManager>();
            raycastManager = GetComponent<ARRaycastManager>();
            furniturePool = new GameObject("Furniture");
            furniturePool.transform.parent = transform;
        }

        void Update()
        {
            Ray touchRay = CheckTouch();
            ScaleSlider();
            HeightSlider();
            if (touchRay.origin != Vector3.zero)
            {
                labelReset = 3.0f;
                PlaceFurniture(touchRay);
            }
            labelReset -= Time.deltaTime;
            if (labelReset <= 0.0f)
                scaleLabel.color = Color.white;
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        Ray CheckTouch()
        {
            if (Input.touchCount <= 0)
                return new Ray();

            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return new Ray();

            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            return touchRay;
        }

        void PlaceFurniture(Ray raycast)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (
                raycastManager.Raycast(
                    raycast.origin,
                    hits,
                    UnityEngine.XR.ARSubsystems.TrackableType.Planes
                )
            )
            {
                if (
                    planeManager.GetPlane(hits[0].trackableId).alignment
                    == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp
                )
                {
                    GameObject newAnchor = GameObject.Instantiate(
                        furnitureToPlace,
                        new Vector3(
                            hits[0].pose.position.x,
                            hits[0].pose.position.y
                                + (maximumHeightOffset * heightOffsetPercentage),
                            hits[0].pose.position.z
                        ),
                        hits[0].pose.rotation,
                        furniturePool.transform
                    );
                    newAnchor.transform.localScale *= maximumScale * scalePercentage;
                    newAnchor.AddComponent<ARAnchor>();
                    scaleLabel.color = Color.green;

                    return;
                }
                scaleLabel.color = Color.yellow;
            }
        }

        public void SelectPrefab(int index)
        {
            if (index < 0 || index > furnitures.Count)
            {
                Debug.LogError("Selected Prefab Index Out of Bounds");
                return;
            }

            furnitureToPlace = furnitures[index];
        }

        public void DeleteAllFurnitures()
        {
            foreach (Transform furniture in furniturePool.transform)
                Destroy(furniture.gameObject);
        }

        public void DeleteAllPlanes()
        {
            foreach (ARPlane plane in gameObject.GetComponentsInChildren<ARPlane>())
                Destroy(plane.gameObject);
        }

        public void TogglePointCloud(bool enabled)
        {
            foreach (
                ARPointCloudParticleVisualizer pointCloud in gameObject.GetComponentsInChildren<ARPointCloudParticleVisualizer>()
            )
                pointCloud.enabled = enabled;
        }

        public void TogglePlanes(bool enabled)
        {
            foreach (
                ARPlaneMeshVisualizer arPlane in gameObject.GetComponentsInChildren<ARPlaneMeshVisualizer>()
            )
                arPlane.enabled = enabled;
        }

        void ScaleSlider()
        {
            if (scaleSlider == null || scaleLabel == null)
                return;
            scalePercentage = scaleSlider.value;
            scaleLabel.text = (
                Mathf.Clamp(maximumScale * scalePercentage, 0.1f, maximumScale)
            ).ToString();
        }

        void HeightSlider()
        {
            if (heightSlider == null || heightLabel == null)
                return;
            heightOffsetPercentage = heightSlider.value;
            heightLabel.text = (
                Mathf.Clamp(maximumHeightOffset * heightOffsetPercentage, 0.0f, maximumHeightOffset)
            ).ToString();
        }
    }
}

using System.Collections.Generic;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace LilLycanLord_Official
{
    public class TrackedImageObjectsManager : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        [SerializeField]
        GameObject noTrackableImageVisiblePrompt;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [SerializeField]
        List<GameObject> trackedImagesWithPrefabs;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        GameObject attachedPrefab;

        [SerializeField]
        bool requireVisibleTrackableImage = false;

        [SerializeField]
        float scaleMultiplier = 1.0f;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        UnityEvent noVisibleTrackabledImageEvent;

        [SerializeField]
        UnityEvent visibleTrackabledImageEvent;

        [SerializeField]
        bool detachPrefabsOnLimitedTracking = false;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        [HideInInspector]
        public bool visibleTrackableImage = true;
        bool previousState = true;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            noTrackableImageVisiblePrompt?.SetActive(requireVisibleTrackableImage);
        }

        void Update()
        {
            if (requireVisibleTrackableImage)
                visibleTrackableImage = trackedImagesWithPrefabs.Count > 0;

            if (visibleTrackableImage != previousState)
                if (visibleTrackableImage)
                    visibleTrackabledImageEvent.Invoke();
                else
                    noVisibleTrackabledImageEvent.Invoke();
            noTrackableImageVisiblePrompt?.SetActive(!visibleTrackableImage);
            previousState = visibleTrackableImage;
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        // public void OnARTrackablesChange(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
        // {
        //     foreach (ARTrackedImage addedImage in eventArgs.added)
        //     {
        //         AttachPrefab(addedImage.transform);
        //     }
        //     foreach (ARTrackedImage updatedImage in eventArgs.updated)
        //     {
        //         Debug.Log(updatedImage.name + " -> " + updatedImage.trackingState);
        //         if (!detachPrefabsOnLimitedTracking)
        //             break;
        //         if (
        //             updatedImage.trackingState == TrackingState.Limited
        //             || updatedImage.trackingState == TrackingState.None
        //         )
        //             DetachPrefab(updatedImage.transform);
        //         else if (updatedImage.transform.Find(attachedPrefab.name) == null)
        //             AttachPrefab(updatedImage.transform);
        //     }
        //     foreach (ARTrackedImage removedImage in eventArgs.removed) { }
        // }

        void AttachPrefab(Transform parent)
        {
            GameObject prefab = GameObject.Instantiate(attachedPrefab, parent);
            prefab.name = attachedPrefab.name;
            prefab.transform.localScale *= scaleMultiplier;
            prefab.transform.localPosition += offset;
            trackedImagesWithPrefabs.Add(parent.gameObject);
            Debug.Log("\"" + parent.name + "\" now has " + parent.childCount + " children");
        }

        void DetachPrefab(Transform parent)
        {
            Destroy(parent.Find(attachedPrefab.name).gameObject);
            trackedImagesWithPrefabs.Remove(parent.gameObject);
            Debug.Log("\"" + parent.name + "\" now has " + parent.childCount + " children");
        }

        //* ╔═════════════════════╗
        //* ║ Temporary Functions ║
        //* ╚═════════════════════╝
        public void StopAllNavAgents()
        {
            foreach (
                UnityEngine.Object navAgentManager in FindObjectsByType(
                    typeof(NavAgentManager),
                    FindObjectsSortMode.InstanceID
                )
            )
                navAgentManager.GetComponent<NavAgentManager>().StopAllAgents();
        }
    }
}

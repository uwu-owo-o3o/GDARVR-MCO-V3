using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace LilLycanLord_Official
{
    public class VirtualButton : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        [SerializeField]
        ARTrackedImage trackedImage;

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
        UnityEvent onTrackedImageLimited;

        [SerializeField]
        bool singlePress = true;

        [SerializeField]
        bool logEvents = true;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        bool onTrackedImageLimitedFired = false;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            trackedImage = GetComponentInParent<ARTrackedImage>();
            if (trackedImage == null)
            {
                Debug.LogError(name + "'s virtual button has no associated tracked image!");
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                if (!singlePress)
                {
                    if (logEvents)
                        Debug.Log(gameObject.name + "'s virtual button held");
                    onTrackedImageLimited.Invoke();
                }
                else
                {
                    if (!onTrackedImageLimitedFired)
                    {
                        if (logEvents)
                            Debug.Log(gameObject.name + "'s virtual button pressed");
                        onTrackedImageLimited.Invoke();
                        onTrackedImageLimitedFired = true;
                    }
                }
            else if (singlePress)
            {
                if (logEvents)
                    Debug.Log(gameObject.name + "'s virtual button unpressed");
                onTrackedImageLimitedFired = false;
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
    }
}

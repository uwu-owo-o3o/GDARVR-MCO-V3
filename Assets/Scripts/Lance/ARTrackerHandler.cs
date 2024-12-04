using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using LilLycanLord_Official;

public class TrackHandler : MonoBehaviour
{
    public Text debugText;
    public List<GameObject> roomPrefabs = new List<GameObject>();

    public void Start()
    {
        this.debugText.text = "NONE";
    }

    public void OnTrackedImageChange(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
        {
            Debug.Log("image name: " + image.referenceImage.name);
            this.checkImageScanned(image.referenceImage.name);
            //Debug.Log("image type:" + image.gameObject.GetType());

        }

        foreach (var image in args.updated)
        {
            //Debug.Log("Updated Image: " + image.referenceImage.name + " | Tracking State: " + image.trackingState);
        }

        foreach (var image in args.removed)
        {
            //Debug.Log("Removed Image: " + image.referenceImage.name + " | Tracking State: " + image.trackingState);
        }
    }


    public void checkImageScanned(string name)
    {
        if (name.Contains("kitchen"))
        {
            this.debugText.text = "kitchen scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("dining"))
        {
            this.debugText.text = "dining scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("guesthouse"))
        {
            this.debugText.text = "guesthouse scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("patio"))
        {
            this.debugText.text = "patio scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("spa"))
        {
            this.debugText.text = "spa scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("theatre"))
        {
            this.debugText.text = "theatre scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("living"))
        {
            this.debugText.text = "living scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("observatory"))
        {
            this.debugText.text = "observatory scanned!";
            this.callTaskManagerForTask();
        }
        else if (name.Contains("cat"))
        {
            this.debugText.text = "cat scanned!";
            this.callTaskManagerForTask();
        }

        this.debugText.text = "NONE";

    }

    public void callTaskManagerForTask()
    {
        TaskManager.Instance.GetTask();
    }
}

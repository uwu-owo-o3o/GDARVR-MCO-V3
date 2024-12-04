using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

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
        }
        else if (name.Contains("dining"))
        {
            this.debugText.text = "dining scanned!";
        }
        else if (name.Contains("guesthouse"))
        {
            this.debugText.text = "guesthouse scanned!";
        }
        else if (name.Contains("patio"))
        {
            this.debugText.text = "patio scanned!";
        }
        else if (name.Contains("spa"))
        {
            this.debugText.text = "spa scanned!";
        }
        else if (name.Contains("theatre"))
        {
            this.debugText.text = "theatre scanned!";
        }
        else if (name.Contains("living"))
        {
            this.debugText.text = "living scanned!";
        }
        else if (name.Contains("observatory"))
        {
            this.debugText.text = "observatory scanned!";
        }
        else if (name.Contains("cat"))
        {
            this.debugText.text = "cat scanned!";
        }

        this.debugText.text = "NONE";

    }
}

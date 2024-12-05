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
    public List<GameObject> spawnedRooms = new List<GameObject>();

    public void Start()
    {
        this.debugText.text = "NONE";
        for (int i = 0; i < this.roomPrefabs.Count; i++)
        {
            this.roomPrefabs[i].SetActive(false);
        }
    }

    public void OnTrackedImageChange(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
        {
            Debug.Log("image name: " + image.referenceImage.name);
            this.checkImageScanned(image.referenceImage.name, image.gameObject);
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


    public void checkImageScanned(string name, GameObject arImage)
    {
        if (name.Contains("kitchen"))
        {
            this.debugText.text = "kitchen scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(0, arImage);
        }
        else if (name.Contains("dining"))
        {
            this.debugText.text = "dining scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(1, arImage);
        }
        else if (name.Contains("guesthouse"))
        {
            this.debugText.text = "guesthouse scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(2, arImage);
        }
        else if (name.Contains("patio"))
        {
            this.debugText.text = "patio scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(3, arImage);
        }
        else if (name.Contains("hall"))
        {
            this.debugText.text = "patio scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(4, arImage);
        }
        else if (name.Contains("spa"))
        {
            this.debugText.text = "spa scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(5, arImage);
        }
        else if (name.Contains("theatre"))
        {
            this.debugText.text = "theatre scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(6, arImage);
        }
        else if (name.Contains("living"))
        {
            this.debugText.text = "living scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(7, arImage);
        }
        else if (name.Contains("observatory"))
        {
            this.debugText.text = "observatory scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(8, arImage);
        }
        else if (name.Contains("cat"))
        {
            this.debugText.text = "cat scanned!";
        }

        this.debugText.text = "NONE";
     
    }

    public void callTaskManagerForTask()
    {
        //TaskManager.Instance.GetTask();
    }

    public void spawnRoom(int index, GameObject arImage)
    {
        if (this.spawnedRooms.Count > 1)
        {
            destroySpawnedObjects();
        }
        else
        {
            GameObject roomspawned = Instantiate(this.roomPrefabs[index]);
            //roomspawned.transform.position = arImage.transform.position;
            this.spawnedRooms.Add(roomspawned);
            
        }
       
    }

    public void destroySpawnedObjects()
    {   
        List<GameObject> copy = new List<GameObject>();
        copy = spawnedRooms;

        for (int i = 0; i < copy.Count; i++)
        {
            Destroy(this.spawnedRooms[i]);
        }
    }

}

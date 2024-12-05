using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using LilLycanLord_Official;

public class TrackHandler : MonoBehaviour
{
    public Text debugText;
    //public Text debugText2;
    public List<GameObject> roomPrefabs = new List<GameObject>();
    public List<GameObject> spawnedRooms = new List<GameObject>();
    public bool goScanFlag = false;
    public GameObject okButton;

    public void Start()
    {
        this.debugText.text = "NONE";
        for (int i = 0; i < this.roomPrefabs.Count; i++)
        {
            this.roomPrefabs[i].SetActive(false);
        }

        this.okButton.SetActive(false);
        this.goScanFlag = true;
    }

    public void OnTrackedImageChange(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (!this.goScanFlag)
        {
            return;    
        }

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
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("dining"))
        {
            this.debugText.text = "dining scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(1, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("guesthouse"))
        {
            this.debugText.text = "guesthouse scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(2, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("patio"))
        {
            this.debugText.text = "patio scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(3, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("hall"))
        {
            this.debugText.text = "patio scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(4, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("spa"))
        {
            this.debugText.text = "spa scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(5, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("theatre"))
        {
            this.debugText.text = "theatre scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(6, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("living"))
        {
            this.debugText.text = "living scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(7, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("observatory"))
        {
            this.debugText.text = "observatory scanned!";
            this.callTaskManagerForTask();
            this.spawnRoom(8, arImage);
            this.goScanFlag = false;
            this.okButton.SetActive(true);
        }
        else if (name.Contains("cat"))
        {
            this.debugText.text = "cat scanned!";
        }

        //this.debugText.text = "NONE";
     
    }

    public void callTaskManagerForTask()
    {
        TaskManager.Instance.GetTask(this.debugText);
    }

    public void spawnRoom(int index, GameObject arImage)
    {
        //this.debugText2.text = "count: " + this.spawnedRooms.Count;
        if (this.spawnedRooms.Count == 0)
        {
            //this.roomPrefabs[index].SetActive(true);
            GameObject roomspawned = Instantiate(this.roomPrefabs[index]);
            roomspawned.SetActive(true);
            roomspawned.transform.parent = null;
            roomspawned.transform.position += new Vector3(0, -1, 0.1f);
            //roomspawned.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
            //this.debugText2.text = "pos: " + roomspawned.transform.position.x + " " + roomspawned.transform.position.z + " " + roomspawned.transform.position.y; 
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

        this.spawnedRooms.Clear();
    }

    public void pressOkButton()
    {
        //this.debugText.text = "pressed ok!";
        this.destroySpawnedObjects();
        this.goScanFlag = true;
        //this.debugText2.text = "flag: " + goScanFlag.ToString();
        this.okButton.SetActive(false);
    }
}

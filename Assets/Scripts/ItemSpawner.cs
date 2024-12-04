using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> items;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private GameObject spawnPoint2;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Randomize()
    {
        int randNum = Random.Range(0, 2);
        Debug.Log(randNum);
        int randItem;

        if (randNum == 1)
        {
            int randItem2;
            randItem = Random.Range(0, items.Count);
            int randNum2 = Random.Range(0, items.Count);
            randItem2 = Random.Range(0, randNum2);
            Spawn2(randItem, randItem2);
        }
        else
        {
            randItem = Random.Range(0, items.Count);
            Spawn1(randItem);
        }

    }
    public void Spawn1(int randItem)
    {
        items[randItem].transform.position = spawnPoint.transform.position;
        items[randItem].SetActive(true);
        Debug.Log("Spawned 1: " + items[randItem]);
    }

    public void Spawn2(int randItem, int randItem2)
    {
        items[randItem].transform.position = spawnPoint.transform.position;
        items[randItem].SetActive(true);
        items[randItem2].transform.position = spawnPoint2.transform.position;
        items[randItem2].SetActive(true);
        Debug.Log("Spawned 2: " + items[randItem]);
        Debug.Log(items[randItem2]);
    }

    public void Reset()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(false);
        }
    }
}

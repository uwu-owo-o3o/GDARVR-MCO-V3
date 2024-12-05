using System.Collections;
using System.Collections.Generic;
using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoHome : MonoBehaviour
{   
    public void goBack()
    {
        SceneManager.LoadScene("UI");
    }
    
    public void debugGetTask()
    {
        //TaskManager.Instance.GetTask();
    }
}

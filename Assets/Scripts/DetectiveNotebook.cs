using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectiveNotebook : MonoBehaviour
{
    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Detective Scene")
        {
            this.gameObject.SetActive(true);
        }
    }
}

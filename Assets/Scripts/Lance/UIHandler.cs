using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject MenuScreen;
    public GameObject InfoBtn;
    public GameObject Scoresheet;
    public GameObject AccuseScreen;
    public GameObject GameProfile;

    private void Start()
    {
       this.TitleScreen.SetActive(true);

       this.MenuScreen.SetActive(false);
       //this.InfoBtn.SetActive(false);

       this.Scoresheet.SetActive(false);
       this.AccuseScreen.SetActive(false);

       this.GameProfile.SetActive(false);
    }

    public void openMenuScreen()
    {
        this.TitleScreen.SetActive(false);

        this.MenuScreen.SetActive(true);
        //this.InfoBtn.SetActive(true);   

        this.Scoresheet.SetActive(false);
        this.AccuseScreen.SetActive(false);

        this.GameProfile.SetActive(false);
    }

    public void openScoresheet()
    {
        this.TitleScreen.SetActive(false);

        this.MenuScreen.SetActive(false);
        //this.InfoBtn.SetActive(true);

        this.Scoresheet.SetActive(true);
        this.AccuseScreen.SetActive(false);

        this.GameProfile.SetActive(false);
    }

    public void openAccuseSCreen()
    {
        this.TitleScreen.SetActive(false);

        this.MenuScreen.SetActive(false);
        //this.InfoBtn.SetActive(true);

        this.Scoresheet.SetActive(false);
        this.AccuseScreen.SetActive(true);

        this.GameProfile.SetActive(false);
    }

    public void openGameProfile()
    {
        this.TitleScreen.SetActive(false);

        this.MenuScreen.SetActive(false);
        //this.InfoBtn.SetActive(true);

        this.Scoresheet.SetActive(false);
        this.AccuseScreen.SetActive(false);

        this.GameProfile.SetActive(true);
    }

    public void openARScene()
    {
        // load ar cam scene here //
        Debug.Log("Opened AR Camera Scene");
        SceneManager.LoadScene("TrackTest");
    }

    public void openTaskScene()
    {
        SceneManager.LoadScene("Tasks");
    }
}

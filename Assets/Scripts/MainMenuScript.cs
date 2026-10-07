using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject ColorPanel;
    public GameObject EditPanel;
    public GameObject SearchGame;

    //private GameObject CurrentPanel;

    public bool EditActive;
    public bool ColorActive;

    public void Play()
    {
        SetPause();
        SearchGame.SetActive(true);
    }

    public void BackFromSearch()
    {
        PauseOff();
        SearchGame.SetActive(false);
    }

    public void SetPause()
    {
        SearchGame.SetActive(true);
        Time.timeScale = 0;
    }

    public void Accept()
    {
        if (EditActive)
        {
            EditPanel.SetActive(false);
            if (ColorActive)
            {
                ColorPanel.SetActive(false);
            }
            EditActive = false;
        }
    }

    public void EditCar()
    {
        if (EditActive)
        {
            EditPanel.SetActive(false);
            if (ColorActive)
            {
                ColorPanel.SetActive(false);

            }
            EditActive = false;
        }
        else
        {
            EditPanel.SetActive(true);
            EditActive = true;
        }
    }

    public void EditColor()
    {
        if (ColorActive)
        {
            ColorPanel.SetActive(false);
            ColorActive = false;
        }
        else
        {
            ColorPanel.SetActive(true);
            ColorActive=true;
        }
    }


    public void PauseOff()
    {
        SearchGame.SetActive(false);
        Time.timeScale = 1;
    }
}

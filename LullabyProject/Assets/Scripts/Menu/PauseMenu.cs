using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using IO.Behaviour;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        var cam = Camera.main;
        Assert.IsNotNull(cam);
        m_camLook = cam.GetComponent<CamLookMouse>();
        Assert.IsNotNull(m_camLook);
        Assert.IsNotNull(pauseMenu);
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_pausedActive) 
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }  
    }

    public void PauseGame() 
    {
        m_camLook.enabled = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        m_pausedActive = true;
    }

    public void ResumeGame() 
    {
        m_camLook.enabled = true;
        Time.timeScale = 1f;
        m_pausedActive = false;
        pauseMenu.SetActive(false);
    }

    public void ReturnToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    CamLookMouse m_camLook;
    bool m_pausedActive;
}

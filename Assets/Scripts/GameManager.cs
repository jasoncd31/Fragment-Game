using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Scene currentScene;
    private bool paused = false;
    private bool instructionsPresent = false;

    public GameObject instructionPanel;
    public GameObject buttonPanel;
    public GameObject escapeMenuCanvas;
    public GameObject mainMenuCanvas;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene is '" + currentScene.name + "'.");
        escapeMenuCanvas.GetComponent<Canvas>().enabled = false;
        instructionPanel.SetActive(false);
        if (!currentScene.name.Equals("MainMenu"))
        {
            mainMenuCanvas.GetComponent<Canvas>().enabled = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !currentScene.name.Equals("MainMenu"))
        {
            GoToMainMenu();
        }
    }
    void PauseGame()
    {
        if (paused)
        {
            escapeMenuCanvas.GetComponent<Canvas>().enabled = false;
            paused = false;
            Time.timeScale = 1f;
        }
        else
        {
            escapeMenuCanvas.GetComponent<Canvas>().enabled = true;
            paused = true;
            Time.timeScale = 0f;
        }
    }
    public void PlayLevel()
    {
        SceneManager.LoadScene("DemoScene");
    }
    public void GoToMainMenu()
    {
        Debug.Log("hi mom");
        SceneManager.LoadScene("MainMenu");
    }
    public void TestText()
    {
        Debug.Log("hey look ma i made it");
    }
    public void Instructions()
    {
        if (instructionsPresent)
        {
            instructionPanel.SetActive(false);
            buttonPanel.SetActive(true);
        }
        else
        {
            instructionPanel.SetActive(true);
            buttonPanel.SetActive(false);
        }
        instructionsPresent = !instructionsPresent;
    }
    public void QuitGame()
    {
        Debug.Log("bye bye");
        Application.Quit();
    }
}

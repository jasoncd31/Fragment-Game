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

    public List<AudioClip> music = new List<AudioClip>();
    public AudioSource audio;

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
        audio = GetComponent<AudioSource>();

        if (!currentScene.name.Equals("MainMenu"))
        {
            mainMenuCanvas.GetComponent<Canvas>().enabled = false;
        }
        MusicPlayer();
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !currentScene.name.Equals("MainMenu"))
        {
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("J");
            StaticLevel();
        }
    }
    public void PauseGame()
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
        SceneManager.LoadScene("LevelGenerationTest");
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
    public void StaticLevel()
    {
        SceneManager.LoadScene("StaticTestLevel");
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
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
        #endif
        Application.Quit();
    }

    void MusicPlayer()
    {
        if(currentScene.name.Equals("MainMenu"))
        {
            audio.clip = music[0];
            audio.Play();
            return;
        }
        if(currentScene.name.Equals("LevelGenerationTest"))
        {
            audio.clip = music[1];
            audio.Play();
            return;
        }
        if(currentScene.name.Equals("StaticTestLevel"))
        {
            audio.clip = music[2];
            audio.Play();
            return;
        }
    }
}

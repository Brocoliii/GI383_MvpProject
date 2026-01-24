using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("-- Game States --")]
    public bool isGameActive = false;
    public string nextSceneName;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        isGameActive = true;
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Debug.Log("Game Manager: Player Lost!");

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 0;

        
    }

    public void WinStage()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Debug.Log("Game Manager: Stage Cleared!");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
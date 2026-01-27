using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using TMPro;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("-- Game States --")]
    public bool isGameActive = false;
    public string nextSceneName;

    [Header("-- UI References --")]
    public GameObject gameOverUI;
    public TextMeshProUGUI causeText;

    private string deathReason;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);
        StartGame();
    }
    
    public void StartGame()
    {
        isGameActive = true;
    }

    public void GameOver(string reason)
    {
        if (!isGameActive) return;
        isGameActive = false;
        Debug.Log("Game Manager: Player Lost!");
        deathReason = reason;
        StartCoroutine(GameOverSequence());
    }

    public void Update()
    {

        

        if (isGameActive)
        {
            GameStats.TotalTimeSpent += Time.deltaTime;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static class GameStats
    {
        public static float TotalTimeSpent = 0f;

        public static void ResetStats()
        {
            TotalTimeSpent = 0f;
        }
    }
    IEnumerator GameOverSequence()
    {
        Debug.Log("1. Start waiting...");
        yield return new WaitForSecondsRealtime(3f);

        Debug.Log("2. Time Scale set to 0");
        Time.timeScale = 0;

        if (gameOverUI != null)
        {
            Debug.Log("3. Opening UI!");
            gameOverUI.SetActive(true);

            if (causeText != null)
            {
                causeText.text = deathReason;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void WinStage()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Debug.Log("Game Manager: Stage Cleared!");
        if (!string.IsNullOrEmpty("EndScene"))
        {
            SceneManager.LoadScene("EndScene");
        }
    }

   
}
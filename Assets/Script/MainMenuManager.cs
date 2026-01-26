using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("-- References --")]
    public VideoPlayer introVideo;
    public GameObject mainMenuUI;
    public GameObject skipUipanel;

    [Header("-- Skip Visual --")]
    public Image skipProgressBar;
    public float holdDuration = 2.0f;

    private static bool hasPlayedVideo = false;

    private float currentHoldTime = 0f;
    private bool isSkippingProcess = false;

    void Start()
    {
        mainMenuUI.SetActive(true);
        if (skipUipanel != null) skipUipanel.SetActive(false);
        skipProgressBar.fillAmount = 0f;

        introVideo.loopPointReached += OnVideoFinished;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (!introVideo.isPlaying) return;

        if (Input.anyKey)
        {
            if (skipUipanel != null) skipUipanel.SetActive(true);
            currentHoldTime += Time.deltaTime;
            skipProgressBar.fillAmount = currentHoldTime / holdDuration;

            if (currentHoldTime >= holdDuration)
            {
                LoadGameScene();
            }
        }
        else
        {
            currentHoldTime = 0f;
            skipProgressBar.fillAmount = 0f;
            if (skipUipanel != null) skipUipanel.SetActive(false);
        }
    }

    public void OnPlayButtonClick()
    {
        if (!hasPlayedVideo)
        {
            StartVideo();
        }
        else
        {
            LoadGameScene();
        }
    }

    void StartVideo()
    {
        hasPlayedVideo = true; 
        mainMenuUI.SetActive(false); 
        introVideo.Play(); 
        Cursor.visible = false; 
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadGameScene();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;


public class MainMenuManager : MonoBehaviour
{
    [Header("-- References --")]
    public VideoPlayer introVideo;
    public GameObject mainMenuUI;
    public GameObject skipUipanel;

    [Header("-- Skip Visual --")]
    public Image skipProgressBar;
    public float holdDuration = 2.0f;

    [Header("-- Fade Settings --")]
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private bool isFading = false;


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

    IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        SceneManager.LoadScene("GamePlayScene");
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
        if (isFading) return;
        StartCoroutine(FadeAndLoadScene());
    }


    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}



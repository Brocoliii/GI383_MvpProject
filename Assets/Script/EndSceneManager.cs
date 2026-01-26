using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
public class EndSceneManager : MonoBehaviour
{
    [Header("-- References --")]
    public VideoPlayer endViedo;
    public GameObject endResultUI;
    public GameObject skipUipanel;

    [Header("-- Skip Visual --")]
    public Image skipProgressBar;
    public float holdDurarion = 2.0f;

    private float currentHoldTime = 0f;
    private bool isSkipped = false;

     void Start()
    {
        if (skipUipanel != null ) skipUipanel.SetActive(false);
        if (endResultUI != null) endResultUI.SetActive(false);
        skipProgressBar.fillAmount = 0f;

        endViedo.loopPointReached += OnVideoFinished;
        endViedo.Play();

        Cursor.visible = false;


    }

     void Update()
    {
        if (isSkipped || !endViedo.isPlaying) return;
        if (Input.anyKey)
        {
            if (skipUipanel != null) skipUipanel.SetActive(true);
            currentHoldTime += Time.deltaTime;
            skipProgressBar.fillAmount = currentHoldTime / holdDurarion;

            if (currentHoldTime >= holdDurarion)
                SkipEndCutScene();
        }
        else
        {
            currentHoldTime = 0f;
            skipProgressBar.fillAmount = 0f;
            if (skipUipanel != null) skipUipanel.SetActive(false);
        }

    }

    public void SkipEndCutScene()
    {
        isSkipped = true;
        endViedo.Stop();
        ShowEndResult();
    }

    void OnVideoFinished (VideoPlayer vp)
    {
        ShowEndResult();
    }

    void ShowEndResult()
    {
        if (skipUipanel != null) skipUipanel.SetActive(false);
        if (endResultUI != null) endResultUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

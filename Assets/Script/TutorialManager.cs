using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("-- Pages --")]
    public GameObject[] pages; 
    private int currentIndex = 0;
    public GameObject tutorialUI;

    [Header("-- Buttons --")]
    public Button btnNext;
    public Button btnBack;  
    public Button btnStart;

    void Start()
    {
        if (PlayerPrefs.GetInt("HasSeenTutorial", 0) == 1)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            if (tutorialUI != null) tutorialUI.SetActive(false); // 2. สั่งปิด UI จริงๆ
            gameObject.SetActive(false);
            return;
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        currentIndex = 0;
        UpdateUI();
    }

    public void NextPage()
    {
        if (currentIndex < pages.Length - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void BackPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == currentIndex);
        }

        if (btnNext != null) btnNext.gameObject.SetActive(currentIndex < pages.Length - 1);
        if (btnBack != null) btnBack.gameObject.SetActive(currentIndex > 0);

        if (btnStart != null) btnStart.gameObject.SetActive(currentIndex == pages.Length - 1);
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("HasSeenTutorial", 1);
        PlayerPrefs.Save();
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (GameObject p in pages )
        {
            if (p != null) p.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
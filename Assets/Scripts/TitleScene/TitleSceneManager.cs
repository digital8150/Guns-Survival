using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [Header("로드할 씬")]
    public string mainGameScene;

    [Header("튜토리얼 이미지")]
    public GameObject tutorialPanel;

    void Start()
    {
        if(tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void Update()
    {
        //ESC 누르면 튜토리얼 창 닫기
        if(tutorialPanel != null && tutorialPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                tutorialPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(mainGameScene);
    }
    public void ShowTutorial()
    {
        if(tutorialPanel != null)
            tutorialPanel.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [Header("�ε��� ��")]
    public string mainGameScene;

    [Header("Ʃ�丮�� �̹���")]
    public GameObject tutorialPanel;

    void Start()
    {
        if(tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void Update()
    {
        //ESC ������ Ʃ�丮�� â �ݱ�
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

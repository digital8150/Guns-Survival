using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeadScene : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup gameOverPanel;

    [SerializeField]
    private float fadeDuration = 2.0f;

    void Start()
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.alpha = 0;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
        }
    }

    public void HandlePlayerDead()
    {
        StartCoroutine(ShowGameOverPanelAfterDelay());
    }

    private IEnumerator ShowGameOverPanelAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        float elapsedTime = 0f;

        while(elapsedTime < fadeDuration)
        {
            if(gameOverPanel != null)
                gameOverPanel.alpha = Mathf.Clamp01(elapsedTime/fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //���̵� �� �Ϸ� �� ��ȣ�ۿ� ����
        if(gameOverPanel != null)
        {
            gameOverPanel.alpha = 1;
            gameOverPanel.interactable = true;
            gameOverPanel.blocksRaycasts = true;
        }
    }

    //------------------------ ��ư �޼ҵ� ----------------------------
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToMain()
    {
        //�̸��� ������ �ʾ� �׳� �ϵ��ڵ� ó��
        SceneManager.LoadScene("TitleScene");
    }
    public void ExitToBackGround()
    {
        Application.Quit();
    }
}

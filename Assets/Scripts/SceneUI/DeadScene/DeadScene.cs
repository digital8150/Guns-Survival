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

        //페이드 인 완료 후 상호작용 가능
        if(gameOverPanel != null)
        {
            gameOverPanel.alpha = 1;
            gameOverPanel.interactable = true;
            gameOverPanel.blocksRaycasts = true;
        }
    }

    //------------------------ 버튼 메소드 ----------------------------
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToMain()
    {
        //이름이 변하지 않아 그냥 하드코딩 처리
        SceneManager.LoadScene("TitleScene");
    }
    public void ExitToBackGround()
    {
        Application.Quit();
    }
}

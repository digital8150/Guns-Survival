using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine.UI;

public class DeadScene : MonoBehaviour
{
    [Header("DeadSceneUI")]
    [SerializeField]
    private CanvasGroup gameOverPanel;
    [Header("페이드 인 시간")]
    [SerializeField]
    private float fadeDuration = 2.0f;
    [Header("플레이 한 시간")]
    [SerializeField]
    private Text playTimeText;

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

        //플레이 시간 표시, 게임 정지, 마우스 활성화
        if (playTimeText != null)
        {
            float timeInSec = Time.timeSinceLevelLoad;
            int minutes = Mathf.FloorToInt(timeInSec / 60F);
            int seconds = Mathf.FloorToInt(timeInSec % 60F);
            playTimeText.text = string.Format("생존 시간 : {0:00}:{1:00}", minutes, seconds);
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //------------------------ 버튼 메소드 ----------------------------
    public void ExitToMain()
    {
        //이름이 변하지 않아 그냥 하드코딩 처리
        SceneManager.LoadScene("TitleScene");
    }
    public void ExitToBackGround()
    {
        Application.Quit();
    }

    //---------------------------- 플레이어 죽음 이벤트 처리 ------------------------
    private void OnEnable()
    {
        HealthController.OnPlayerDied += HandlePlayerDead;
    }
    private void OnDisable()
    {
        HealthController.OnPlayerDied -= HandlePlayerDead;
    }
}

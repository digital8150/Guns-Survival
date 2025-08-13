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
    [Header("���̵� �� �ð�")]
    [SerializeField]
    private float fadeDuration = 2.0f;
    [Header("�÷��� �� �ð�")]
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

        //���̵� �� �Ϸ� �� ��ȣ�ۿ� ����
        if(gameOverPanel != null)
        {
            gameOverPanel.alpha = 1;
            gameOverPanel.interactable = true;
            gameOverPanel.blocksRaycasts = true;
        }

        //�÷��� �ð� ǥ��, ���� ����, ���콺 Ȱ��ȭ
        if (playTimeText != null)
        {
            float timeInSec = Time.timeSinceLevelLoad;
            int minutes = Mathf.FloorToInt(timeInSec / 60F);
            int seconds = Mathf.FloorToInt(timeInSec % 60F);
            playTimeText.text = string.Format("���� �ð� : {0:00}:{1:00}", minutes, seconds);
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //------------------------ ��ư �޼ҵ� ----------------------------
    public void ExitToMain()
    {
        //�̸��� ������ �ʾ� �׳� �ϵ��ڵ� ó��
        SceneManager.LoadScene("TitleScene");
    }
    public void ExitToBackGround()
    {
        Application.Quit();
    }

    //---------------------------- �÷��̾� ���� �̺�Ʈ ó�� ------------------------
    private void OnEnable()
    {
        HealthController.OnPlayerDied += HandlePlayerDead;
    }
    private void OnDisable()
    {
        HealthController.OnPlayerDied -= HandlePlayerDead;
    }
}

using System;
using UnityEngine;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine.SceneManagement;
using System.Collections;

//�ð��� �����ϴ� ��ũ��Ʈ
//�ð��� ���ߴ� ��ũ��Ʈ�� TimeScaleManager�� ����
public class TimeManager : MonoBehaviour
{
    [Header("���� �ð� �ѵ�")]
    [SerializeField]
    private float ClearTime;
    [Header("���� Ŭ���� ����")]
    [SerializeField]
    private int clearScore;
    public float ElapsedTime {  get; private set; }
    public static event Action<float> OnTimeChanged;

    private bool isGameCleared = false;     //���� Ŭ���� ����

    private void Start()
    {
        ElapsedTime = 0f;
    }
    void Update()
    {
        if (isGameCleared) return;

        ElapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke(ElapsedTime);

        if (ElapsedTime >= ClearTime)
            GameClear();
    }

    private void OnEnable()
    {
        HealthController.OnPlayerDied += PlayerDeathSq;
        UIManager.OnFinalBossDied += GameClear;
    }
    private void OnDisable()
    {
        HealthController.OnPlayerDied -= PlayerDeathSq;
        UIManager.OnFinalBossDied -= GameClear;
    }

    private void PlayerDeathSq()
    {
        PlayerPrefs.SetFloat("SurvivalTime", ElapsedTime);
        PlayerPrefs.SetInt("PlayerScore", GameBuilders.FPSBuilder.Core.Managers.GameplayManager.Instance.Score);
        PlayerPrefs.Save();
        StartCoroutine(LoadGameOverSceneAfterDelay(3f));
    }

    private IEnumerator LoadGameOverSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOverScene");
    }

    //-------------------- Game Clear Check ----------------------
    private void GameClear()
    {
        if(isGameCleared) return;
        isGameCleared = true;
        GameBuilders.FPSBuilder.Core.Managers.GameplayManager.Instance.Score += clearScore;
        PlayerPrefs.SetInt("PlayerScore", GameBuilders.FPSBuilder.Core.Managers.GameplayManager.Instance.Score);

        StartCoroutine(LoadGameClearSceneAfterDelay(3f));
    }
    private IEnumerator LoadGameClearSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameClear");
    }
}

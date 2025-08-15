using System;
using UnityEngine;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine.SceneManagement;
using System.Collections;

//시간을 측정하는 스크립트
//시간을 멈추는 스크립트는 TimeScaleManager에 의존
public class TimeManager : MonoBehaviour
{
    [Header("게임 시간 한도")]
    [SerializeField]
    private float ClearTime;
    [Header("게임 클리어 점수")]
    [SerializeField]
    private int clearScore;
    public float ElapsedTime {  get; private set; }
    public static event Action<float> OnTimeChanged;

    private bool isGameCleared = false;     //게임 클리어 여부

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

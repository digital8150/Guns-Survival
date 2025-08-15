using System;
using UnityEngine;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine.SceneManagement;
using System.Collections;

//시간을 측정하는 스크립트
//시간을 멈추는 스크립트는 TimeScaleManager에 의존
public class TimeManager : MonoBehaviour
{
    public float ElapsedTime {  get; private set; }
    public static event Action<float> OnTimeChanged;

    private void Start()
    {
        ElapsedTime = 0f;
    }
    void Update()
    {
        ElapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke(ElapsedTime);
    }

    private void OnEnable()
    {
        HealthController.OnPlayerDied += PlayerDeathSq;
    }
    private void OnDisable()
    {
        HealthController.OnPlayerDied -= PlayerDeathSq;
    }

    private void PlayerDeathSq()
    {
        PlayerPrefs.SetFloat("SurvivalTime", ElapsedTime);
        PlayerPrefs.Save();
        StartCoroutine(LoadGameOverSceneAfterDelay(3f));
    }

    private IEnumerator LoadGameOverSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOverScene");
    }
}

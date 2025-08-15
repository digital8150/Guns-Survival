using System;
using UnityEngine;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine.SceneManagement;
using System.Collections;

//�ð��� �����ϴ� ��ũ��Ʈ
//�ð��� ���ߴ� ��ũ��Ʈ�� TimeScaleManager�� ����
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

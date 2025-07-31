using System.Collections;
using UnityEngine;

/// <summary>
/// Time.timeScale을 부드럽게 변경하여 게임의 시간을 제어
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    [Tooltip("시간 배율이 변경되는 데 걸리는 시간 (초)")]
    public static float transitionDuration = 0.5f;

    private Coroutine _timeScaleCoroutine;

    public void Pause()
    {
        SetTimeScale(0f);
    }

    public  void Resume()
    {
        SetTimeScale(1f);
    }

    public void SetCustomTimeScale(float targetScale)
    {
        SetTimeScale(targetScale);
    }

    private void SetTimeScale(float targetScale)
    {
        if (_timeScaleCoroutine != null)
        {
            StopCoroutine(_timeScaleCoroutine);
        }
        _timeScaleCoroutine = StartCoroutine(TimeScaleCoroutine(targetScale));
    }

    private IEnumerator TimeScaleCoroutine(float targetScale)
    {
        float elapsedTime = 0f;
        float startScale = Time.timeScale;

        while (elapsedTime < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(startScale, targetScale, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetScale;
        _timeScaleCoroutine = null;
    }
}
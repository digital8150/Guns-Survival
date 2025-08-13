using System.Collections;
using UnityEngine;

/// <summary>
/// Time.timeScale�� �ε巴�� �����Ͽ� ������ �ð��� ����
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    [Tooltip("�ð� ������ ����Ǵ� �� �ɸ��� �ð� (��)")]
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
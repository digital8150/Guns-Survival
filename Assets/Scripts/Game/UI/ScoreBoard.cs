using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private List<TextTyping> scoreTexts = new List<TextTyping>();

    [SerializeField]
    private float m_ScoreMoveDuration = 0.2f;
    [SerializeField]
    private float m_ScoreMoveDistance = 30f;

    private int currentIdx = 0;

    private void OnEnable()
    {
        Enemy.OnScoreUp += ScoreUp;
    }

    private void OnDisable()
    {
        Enemy.OnScoreUp -= ScoreUp;
    }

    void ScoreUp(int score, string content)
    {
        ShiftAllScoresDown();
        TextTyping newScoreText = scoreTexts[currentIdx];
        newScoreText.StopAllCoroutines();
        
        newScoreText.gameObject.SetActive(true);
        newScoreText.transform.localPosition = Vector3.zero;
        newScoreText.StartAnimation($"+{score} {content}");
        Debug.Log($"Scoreboard: curredIdx:{currentIdx}, {newScoreText.transform.localPosition}");
        IncreaseIndex();
    }

    void ShiftAllScoresDown()
    {
        foreach (var text in scoreTexts)
        {
            // 비활성화된 텍스트는 움직일 필요가 없습니다.
            if (text.gameObject.activeSelf)
            {
                StartCoroutine(MoveScoreDown(text.gameObject));
            }
        }
    }

    IEnumerator MoveScoreDown(GameObject obj)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = obj.transform.localPosition;
        Vector3 targetPosition = startPosition - new Vector3(0, m_ScoreMoveDistance, 0);

        while (elapsedTime < m_ScoreMoveDuration)
        {
            float t = elapsedTime / m_ScoreMoveDuration;
            t = t * t * (3f - 2f * t); //cubic-ease 적용
            obj.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = targetPosition;
    }

    void IncreaseIndex()
    {
        currentIdx++;
        currentIdx %= scoreTexts.Count;
    }
}
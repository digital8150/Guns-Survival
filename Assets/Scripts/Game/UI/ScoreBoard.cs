using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] List<TextTyping> scoreTexts = new List<TextTyping>();
    [SerializeField] float m_ScoreMoveDuration;
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
        MoveAllScoresDown(scoreTexts[currentIdx]);
        scoreTexts[currentIdx].transform.localPosition= Vector3.zero;
        scoreTexts[currentIdx].StartAnimation($"+{score} {content}");
        IncreaseIndex();
    }

    IEnumerator MoveScoreDown(GameObject obj)
    {
        float moveSpeed = 30f / m_ScoreMoveDuration;
        float elapsedTime = 0f;

        while (elapsedTime < m_ScoreMoveDuration)
        {
            float moveStep = moveSpeed * Time.deltaTime;
            obj.transform.localPosition -= new Vector3(0, moveStep, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void MoveAllScoresDown(TextTyping Except)
    {
        foreach (var obj in scoreTexts)
        {
            if(obj == Except)
            {
                continue;
            }
            StartCoroutine(MoveScoreDown(obj.gameObject));
        }
    }


    void IncreaseIndex()
    {
        currentIdx++;
        currentIdx %= scoreTexts.Count;
    }
}

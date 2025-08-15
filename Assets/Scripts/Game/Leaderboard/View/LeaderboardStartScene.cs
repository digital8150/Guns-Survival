using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;

public class LeaderboardStartScene : MonoBehaviour
{
    [SerializeField]
    List<ScoreEntryUI> scoreEntries;

    private LeaderboardHttpService leaderboardHttpService;

    private void Start()
    {
        leaderboardHttpService = new LeaderboardHttpService("https://oci.codingbot.kr", this);
        FetchTop10Scores();
    }

    void FetchTop10Scores()
    {
        leaderboardHttpService.GetTop10(onSuccess: (scores) =>
            {
                UpdateView(scores);
            },
            onError: (error) =>
            {
                Debug.LogError($"점수 불러오기 실패 : {error}");
            }
        );
    }

    void UpdateView(List<ScoreEntry> scores)
    {
        for (int i = 0; i < 10; i++)
        {
            if (i >= scores.Count)
            {
                scoreEntries[i].gameObject.SetActive(false);
                continue;
            }

            scoreEntries[i].text_Nickname.text = scores[i].nickname;
            scoreEntries[i].text_Score.text = scores[i].score.ToString();
            scoreEntries[i].text_SurvivalTime.text = scores[i].survival_time.ToString();
            scoreEntries[i].text_DateTime.text = scores[i].registration_date.ToString();
        }
    }
}

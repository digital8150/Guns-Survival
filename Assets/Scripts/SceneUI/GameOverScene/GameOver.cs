using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Time Text")]
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private InputField usernameInput;


    int score;
    int time;

    void Start()
    {
        //버튼과 상호작용 가능하도록 변경
        Time.timeScale = 1.0f;

        if (timeText != null)
        {
            float ftime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
            time = (int)ftime;

            int m = Mathf.FloorToInt(ftime / 60);
            int s = Mathf.FloorToInt(ftime % 60);
            timeText.text = string.Format("{0:00}:{1:00}",m,s);
        }

        score = PlayerPrefs.GetInt("PlayerScore", 0);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitMain()
    {
        SceneManager.LoadScene("TitleScene");
        if (usernameInput != null && usernameInput.text != "")
        {

            LeaderboardHttpService service = new LeaderboardHttpService("https://oci.codingbot.kr", this);
            service.AddScore(new ScoreCreate(score, time, usernameInput.text),
                onSuccess => 
                { 
                    Debug.Log("점수등록 성공!"); 
                },
                onError: (error) => 
                { 
                    Debug.LogError($"점수 등록 실패! : {error}"); 
                });
        }
    }

    public void ExitBackGround()
    {
        Application.Quit();
    }
}

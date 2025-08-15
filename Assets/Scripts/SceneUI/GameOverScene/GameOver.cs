using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameBuilders.FPSBuilder.Core.Managers;

public class GameOver : MonoBehaviour
{
    [Header("Time Text")]
    [SerializeField]
    private Text timeText;

    void Start()
    {
        if (timeText != null)
        {
            float ftime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

            int m = Mathf.FloorToInt(ftime / 60);
            int s = Mathf.FloorToInt(ftime % 60);
            timeText.text = string.Format("{0:00}:{1:00}",m,s);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitMain()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ExitBackGround()
    {
        Application.Quit();
    }
}

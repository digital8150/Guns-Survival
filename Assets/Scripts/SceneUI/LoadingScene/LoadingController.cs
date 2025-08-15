using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public static string nextScene;

    [Header("로딩 슬라이더")]
    public Slider progressBar;

    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }
    
    //비동기 형식 로딩 메소드
    private IEnumerator LoadSceneProgress()
    {
        yield return null;

        //로딩 진행 상태를 op 변수에 할당
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        //로딩이 90% 되었을때 대기 상태 유지 -> 로딩 제어
        op.allowSceneActivation = false;

        while(!op.isDone)
        {
            yield return null;

            //timer += Time.deltaTime;

            if(op.progress < 0.9f)
            {
                //로딩바 증가
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, Time.deltaTime *2f);
            }
            //로딩바가 끝까지 차면 불러온 씬으로 교체
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, Time.deltaTime * 2f);
                if (progressBar.value >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}

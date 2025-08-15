using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public static string nextScene;

    [Header("�ε� �����̴�")]
    public Slider progressBar;

    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }
    
    //�񵿱� ���� �ε� �޼ҵ�
    private IEnumerator LoadSceneProgress()
    {
        yield return null;

        //�ε� ���� ���¸� op ������ �Ҵ�
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        //�ε��� 90% �Ǿ����� ��� ���� ���� -> �ε� ����
        op.allowSceneActivation = false;

        while(!op.isDone)
        {
            yield return null;

            //timer += Time.deltaTime;

            if(op.progress < 0.9f)
            {
                //�ε��� ����
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, Time.deltaTime *2f);
            }
            //�ε��ٰ� ������ ���� �ҷ��� ������ ��ü
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

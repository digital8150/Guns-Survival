using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTyping : MonoBehaviour
{
    [SerializeField] Text text;
    WaitForSecondsRealtime interval = new WaitForSecondsRealtime(0.05f);

    public void StartAnimation(string content)
    {
        StopAllCoroutines();
        StartCoroutine(RunAnimation(content));
    }

    IEnumerator RunAnimation(string content)
    {
        yield return StartCoroutine(Typing(content));
        yield return StartCoroutine(Deleting());
    }


    IEnumerator Typing(string content)
    {
        text.text = null;
        for(int i=0; i < content.Length; i++)
        {
            text.text += content[i];
            yield return interval;
        }
    }

    IEnumerator Deleting()
    {
        yield return new WaitForSecondsRealtime(2f);
        for (int i = text.text.Length - 1; i >= 0; i--)
        {
            text.text = text.text.Remove(i);
            yield return interval;
        }
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        text.text = null;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTyping : MonoBehaviour
{
    [SerializeField] Text text;
    WaitForSeconds interval = new WaitForSeconds(0.05f);

    public void StartAnimation(string content)
    {
        StopAllCoroutines();
        StartCoroutine(Typing(content));
        StartCoroutine(Deleting());
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
        yield return new WaitForSeconds(2f);
        for (int i = text.text.Length - 1; i >= 0; i--)
        {
            text.text = text.text.Remove(i);
            yield return interval;
        }
    }
}

/*
 * 
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameBuilders.MinimalistUI.Scripts.Tween
{
    public static class SimpleTween
    {
        public static IEnumerator Fade(GameObject gameObject, float duration = 1)
        {
            TextMeshProUGUI[] text = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = gameObject.GetComponentsInChildren<Image>();
            RawImage[] rawImages = gameObject.GetComponentsInChildren<RawImage>();

            for (float t = duration; t >= 0; t -= Time.deltaTime)
            {
                float a = t / duration;
                
                for (int i = 0, l = text.Length; i < l; i++)
                {
                    text[i].color = new Color(text[i].color.r, text[i].color.g, text[i].color.b, text[i].color.a * a);
                }
                
                for (int i = 0, l = images.Length; i < l; i++)
                {
                    images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, images[i].color.a * a);
                }
                
                for (int i = 0, l = rawImages.Length; i < l; i++)
                {
                    rawImages[i].color = new Color(rawImages[i].color.r, rawImages[i].color.g, rawImages[i].color.b, rawImages[i].color.a * a);
                }
                
                yield return null;
            }
        }
        
        public static IEnumerator Reveal(GameObject gameObject, float duration = 1)
        {
            TextMeshProUGUI[] text = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = gameObject.GetComponentsInChildren<Image>();
            RawImage[] rawImages = gameObject.GetComponentsInChildren<RawImage>();

            for (float t = 0; t <= duration; t += Time.deltaTime)
            {
                float a = t / duration;
                
                for (int i = 0, l = text.Length; i < l; i++)
                {
                    text[i].color = new Color(text[i].color.r, text[i].color.g, text[i].color.b, a);
                }
                
                for (int i = 0, l = images.Length; i < l; i++)
                {
                    images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, a);
                }
                
                for (int i = 0, l = rawImages.Length; i < l; i++)
                {
                    rawImages[i].color = new Color(rawImages[i].color.r, rawImages[i].color.g, rawImages[i].color.b, a);
                }
                
                yield return null;
            }
        }

        public static IEnumerator Move(GameObject gameObject, Vector3 destination, float duration = 1, float speed = 1)
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector3 origin = rectTransform.localPosition;

            for (float t = 0; t <= duration; t += Time.deltaTime * speed)
            {
                float by = t / duration;
                
                
                rectTransform.localPosition = Vector3.Lerp(origin, destination, by);
                
                yield return null;
            }
        }
    }
}

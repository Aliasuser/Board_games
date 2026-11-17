using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{

    Image img;
    Color tempColor;


    void Start()
    {
        img = GetComponent<Image>();
        tempColor = img.color;
        tempColor.a = 1f;
        img.color = tempColor;
        StartCoroutine(FadeIn(0.15f));
    }

    public IEnumerator FadeIn(float fadeTime)
    {
        float stepTime = fadeTime / 20f;   // 20 steps of 0.05
        for (float a = 1f; a >= -0.05f; a -= 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(stepTime);
        }
        img.raycastTarget = false;
    }

    public IEnumerator FadeOut(float fadeTime)
    {
        img.raycastTarget = true;
        float stepTime = fadeTime / 20f;   // 20 steps of 0.05
        for (float a = 0f; a <= 1.05f; a += 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(stepTime);
        }
        img.raycastTarget = false;
    }
}
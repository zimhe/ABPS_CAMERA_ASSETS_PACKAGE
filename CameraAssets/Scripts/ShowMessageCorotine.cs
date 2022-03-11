using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMessageCorotine : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public float duration;

    float timeOnEnable;
    float timeElasped;

    public bool startOnEnable = false;
    private void OnEnable()
    {
        print("Enabled " + gameObject.name);
        timeOnEnable = Time.time;
        timeElasped = 0;

        if (startOnEnable)
        {
            StartCoroutine(Show());
        }
    }

    public IEnumerator Show()
    {
        canvasGroup.gameObject.SetActive(true);
        while (timeElasped < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0.2f, 1.0f, Mathf.Sin(timeElasped*5));
            timeElasped = Time.time - timeOnEnable;
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
    }

}

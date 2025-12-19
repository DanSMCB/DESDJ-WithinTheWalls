using UnityEngine;
using TMPro;
using System.Collections;

public class Captions : MonoBehaviour
{
    public GameObject captions;
    private TMP_Text captionText;
    public float captionDuration = 5f;
    Coroutine captionRoutine;

    public void ShowCaption(string text)
    {
        if (captionRoutine != null)
            StopCoroutine(captionRoutine);

        captionRoutine = StartCoroutine(ShowCaptionRoutine(text));
    }

    IEnumerator ShowCaptionRoutine(string text)
    {
        captions.gameObject.SetActive(true);
        captions.GetComponent<TMP_Text>().text=text;

        yield return new WaitForSeconds(captionDuration);

        captions.GetComponent<TMP_Text>().text = "";
        captions.gameObject.SetActive(false);

        captionRoutine = null;
    }
}

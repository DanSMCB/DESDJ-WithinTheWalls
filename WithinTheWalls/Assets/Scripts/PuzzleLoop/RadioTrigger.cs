using UnityEngine;
using System.Collections;

public class RadioTrigger : MonoBehaviour
{
    public AudioSource radio;
    public float fadeDuration = 1.5f;

    Coroutine fadeCoroutine;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Avatar")) return;

        if (!radio.isPlaying)
            radio.Play();

        StartFade(1f);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Avatar")) return;

        StartFade(0f);
    }

    void StartFade(float targetVolume)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetVolume));
    }

    IEnumerator FadeRoutine(float target)
    {
        float start = radio.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            radio.volume = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        radio.volume = target;

        if (target == 0f)
            radio.Stop();
    }
}
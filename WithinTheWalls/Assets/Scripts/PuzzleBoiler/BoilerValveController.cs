using System.Collections;
using UnityEngine;
using Alteruna;

public class BoilerValveController : AttributesSync
{
    [SynchronizableField]
    public bool isOpen = false;

    public AudioClip interactionClip;
    public AudioClip[] steamClips;
    public float minSteamDelay = 3f;
    public float maxSteamDelay = 5f;

    private AudioSource audioSource;
    private bool steamCoroutineRunning;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void InteractValve()
    {
        if (BoilerRoomManager.Instance.LocalPlayerIsAffected())
            return;

        isOpen = !isOpen;
        if (interactionClip)
            audioSource.PlayOneShot(interactionClip);
    }

    void Update()
    {
        UpdateVisuals();
        UpdateSteamLogic();
    }

    void UpdateVisuals()
    {
        Quaternion targetRotation = isOpen
            ? Quaternion.Euler(0, 180, 0)
            : Quaternion.identity;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }

    void UpdateSteamLogic()
    {
        if (isOpen && !steamCoroutineRunning)
        {
            StartCoroutine(PlaySteamSounds());
        }
    }

    IEnumerator PlaySteamSounds()
    {
        steamCoroutineRunning = true;

        while (isOpen && BoilerRoomManager.Instance.LocalPlayerIsAffected())
        {
            if (steamClips.Length > 0)
            {
                var clip = steamClips[Random.Range(0, steamClips.Length)];
                audioSource.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(
                Random.Range(minSteamDelay, maxSteamDelay)
            );
        }

        steamCoroutineRunning = false;
    }
}

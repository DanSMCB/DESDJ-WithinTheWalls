using System.Collections;
using Alteruna;
using UnityEngine;
using System.Collections.Generic;

public class BoilerRoomManager : AttributesSync
{
    [SynchronizableField]
    public ushort affectedUserIndex = ushort.MaxValue;

    public static BoilerRoomManager Instance { get; private set; }
    [Header("Valves Settings")]
    public BoilerValveController[] valves;
    public float openValveRatio = 0.4f;

    [Header("Particles")]
    private ParticleSystem globalSteam;
    private ParticleSystem steam;
    private float fadeOutDuration = 3f;

    private bool puzzleSolved = false;

    private HashSet<User> playersInBasement = new();
    private bool lastAnyValveOpen;

    void Awake()
    {
        Instance = this;
        steam = GetComponentInChildren<ParticleSystem>();
        globalSteam = transform.Find("GlobalSteam")?.GetComponent<ParticleSystem>();
        if (steam == null)
            Debug.LogWarning("Nenhum ParticleSystem encontrado como filho!");
    }

    IEnumerator Start()
    {
        while (Multiplayer.Instance.GetUsers().Count <= 1)
            yield return null;

        PickAffectedPlayer();
        SetupValves();
        UpdateSteamEffects();
    }

    void Update()
    {
        bool anyValveOpen = false;

        foreach (var v in valves)
        {
            if (v.isOpen)
            {
                anyValveOpen = true;
                break;
            }
        }

        if (anyValveOpen != lastAnyValveOpen)
        {
            lastAnyValveOpen = anyValveOpen;
            UpdateSteamEffects();
        }
    }

    void SetupValves()
    {
        int openCount = Mathf.CeilToInt(valves.Length * openValveRatio);
        List<BoilerValveController> shuffled = new(valves);
        shuffled.Sort((a, b) => Random.value.CompareTo(Random.value));
        for (int i = 0; i < shuffled.Count; i++)
            shuffled[i].isOpen = i < openCount;
    }

    void PickAffectedPlayer()
    {
        var users = Multiplayer.Instance.GetUsers();
        if (users.Count == 1) return;

        affectedUserIndex = users[Random.Range(0, users.Count)].Index;

        Debug.Log("Jogador afetado escolhido: " + affectedUserIndex + ", " + users[affectedUserIndex].Name);
    }

    public bool LocalPlayerIsAffected()
    {
        var localUser = Multiplayer.Instance.GetUser();
        return localUser != null &&
               affectedUserIndex != ushort.MaxValue &&
               localUser.Index == affectedUserIndex;
    }

    public void PlayerEnteredBasement(User user)
    {
        playersInBasement.Add(user);
        UpdateSteamEffects();
    }

    public void PlayerExitedBasement(User user)
    {
        playersInBasement.Remove(user);
        UpdateSteamEffects();
    }

    void OnTriggerEnter(Collider other)
    {
        Alteruna.Avatar avatar = other.GetComponentInParent<Alteruna.Avatar>();
        if (avatar?.Owner != null)
            PlayerEnteredBasement(avatar.Owner);
    }

    void OnTriggerExit(Collider other)
    {
        Alteruna.Avatar avatar = other.GetComponentInParent<Alteruna.Avatar>();
        if (avatar?.Owner != null)
            PlayerExitedBasement(avatar.Owner);
    }

    void UpdateSteamEffects()
    {
        var localAvatar = Multiplayer.Instance.GetAvatar();

        bool anyValveOpen = false;
        foreach (var v in valves)
            if (v.isOpen)
                anyValveOpen = true;

        bool shouldShowSteam =
            anyValveOpen &&
            BoilerRoomManager.Instance != null &&
            BoilerRoomManager.Instance.LocalPlayerIsAffected();


        if (steam != null)
        {
            if (shouldShowSteam && !steam.isPlaying)
            {
                // return steam emission rate to normal
                var emission = steam.emission;
                emission.rateOverTime = 350f;
                steam.Play(true);
                globalSteam.Play(true);
            }
            else if (!shouldShowSteam && steam.isPlaying)
            {
                globalSteam.Stop();
                StartCoroutine(FadeOutParticles(steam));
            }

        }
    }

    IEnumerator FadeOutParticles(ParticleSystem ps)
    {
        var emission = ps.emission;
        float startRate = emission.rateOverTime.constant;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(startRate, 0f, t / fadeOutDuration);
            emission.rateOverTime = lerp;
            yield return null;
        }

        emission.rateOverTime = 0f;
        ps.Stop();
    }
}

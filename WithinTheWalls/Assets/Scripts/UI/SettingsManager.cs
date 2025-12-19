using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    private AudioMixer masterMixer;
    private Volume globalVolume;
    private ColorAdjustments colorAdjust;

    [Header("UI Elements")]
    public TMP_Text volumeText;
    public TMP_Text gammaText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            LoadAudioMixer();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindSceneGlobalVolume();
        LoadSettings();
    }

    // -----------------------------------------
    //  LOAD AUDIO MIXER AUTOMATICALLY
    // -----------------------------------------
    void LoadAudioMixer()
    {
        masterMixer = Resources.Load<AudioMixer>("Audio/MasterMixer");

        if (masterMixer == null)
            Debug.LogError("ERROR: MasterMixer not found in Resources/Audio/");
    }

    // -----------------------------------------
    //  FIND GLOBAL VOLUME WHEN SCENE CHANGES
    // -----------------------------------------
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneGlobalVolume();
        ApplyGamma(PlayerPrefs.GetFloat("Gamma", 1f));
        ApplyVolume(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
    }

    void FindSceneGlobalVolume()
    {
        globalVolume = FindAnyObjectByType<Volume>();

        if (globalVolume == null)
        {
            Debug.LogWarning("No Global Volume found in scene!");
            return;
        }

        if (!globalVolume.profile.TryGet(out colorAdjust))
        {
            Debug.LogWarning("Global Volume has no ColorAdjustments override!");
        }
    }

    // -----------------------------------------
    //  SETTINGS LOGIC
    // -----------------------------------------
    public void LoadSettings()
    {
        ApplyVolume(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
        ApplyGamma(PlayerPrefs.GetFloat("Gamma", 1f));
    }

    public void ApplyVolume(float value)
    {
        if (masterMixer == null)
            return;

        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void ApplyGamma(float value)
    {
        if (colorAdjust != null)
            colorAdjust.postExposure.value = value;

        if (gammaText != null)
        {
            float percent = (value + 5) / 4;
            gammaText.text = Mathf.RoundToInt(percent * 100f) + "%";
        }
    }
}

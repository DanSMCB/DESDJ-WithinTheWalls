using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider gammaSlider;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        gammaSlider.value = PlayerPrefs.GetFloat("Gamma", 1f);

        volumeSlider.onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetFloat("MasterVolume", v);
            SettingsManager.Instance.ApplyVolume(v);
        });

        gammaSlider.onValueChanged.AddListener(g =>
        {
            PlayerPrefs.SetFloat("Gamma", g);
            SettingsManager.Instance.ApplyGamma(g);
        });
    }
}

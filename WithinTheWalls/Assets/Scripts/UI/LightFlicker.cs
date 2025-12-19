using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light[] lights; // Array com todas as luzes do objeto

    public float minIntensity = 0.8f;
    public float maxIntensity = 2f;
    public float speed = 10f;

    void Start()
    {
        // Apanha todas as luzes dentro do objeto
        lights = GetComponentsInChildren<Light>();

        if (lights.Length == 0)
            Debug.LogWarning("LightFlicker: Não foram encontradas luzes no objeto.");
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * speed, 0f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Aplica a intensidade a TODAS as luzes
        foreach (var light in lights)
        {
            light.intensity = intensity;
        }
    }
}

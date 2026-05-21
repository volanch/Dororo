using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject pausePanel;

    public void Open()
    {
        gameObject.SetActive(true);
        if (mainMenuButtons != null)
            mainMenuButtons.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (mainMenuButtons != null)
            mainMenuButtons.SetActive(true);
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        float db = value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("MasterVolume", db);
    }

    public void SetShadows(bool enabled)
    {
        var light = FindFirstObjectByType<UnityEngine.Rendering.Universal.Light2D>();
        if (light != null)
            light.intensity = enabled ? 1f : 0.5f;
    }
}
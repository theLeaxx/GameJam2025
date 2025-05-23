using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Toggle vsyncToggle;

    private void Awake()
    {
        Instance = this;

        LoadOptions();
    }

    public void LoadOptions()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100f);
        vsyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;

        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        AudioListener.volume = masterVolumeSlider.value / 100f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateVolume();
        }
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("VSync", vsyncToggle.isOn ? 1 : 0);

        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        AudioListener.volume = masterVolumeSlider.value / 100f;
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateVolume();
        }
    }
}

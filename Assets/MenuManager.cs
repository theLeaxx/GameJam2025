using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuUI;

    [SerializeField]
    private GameObject optionsMenuUI;

    [SerializeField]
    private GameObject creditsMenuUI;

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Toggle vsyncToggle;

    private void Awake()
    {
        LoadOptions();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void BackToMenu()
    {
        creditsMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void ShowOptions()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void ShowCredits()
    {
        mainMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }

    public void LoadOptions()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100f);
        vsyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;

        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        AudioListener.volume = masterVolumeSlider.value / 100f;
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("VSync", vsyncToggle.isOn ? 1 : 0);

        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        AudioListener.volume = masterVolumeSlider.value / 100f;
    }
}

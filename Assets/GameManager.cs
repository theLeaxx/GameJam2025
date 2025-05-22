using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float EnergyLevel { get; private set; } = 1000f;

    [SerializeField] // change into image later
    private Slider energySlider;

    public GameObject Striker;
    public GameObject Defender;

    public float TotalUsedEnergy = 0;

    [SerializeField]
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SaveLoad.Instance.LoadGame();
        SaveLoad.Instance.SaveGame();

        AudioListener.volume = PlayerPrefs.GetFloat("masterVolume", 100f);
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 100f);
    }

    public void UpdateCurrentNavMesh()
    {
        var navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    public void SetEnergy(float value)
    {
        EnergyLevel = value;
        UpdateEnergyVisual();
    }

    private void Update()
    {
        IncreaseEnergy(Time.deltaTime * 0.001f);
    }

    private void UpdateEnergyVisual()
    {
        energySlider.value = EnergyLevel;
    }

    public void DecreaseEnergy(float amount)
    {
        EnergyLevel -= amount;
        if (EnergyLevel < 0)
        {
            EnergyLevel = 0;
        }

        UpdateEnergyVisual();
        TotalUsedEnergy += amount;
    }

    public void IncreaseEnergy(float amount)
    {
        EnergyLevel += amount;
        if (EnergyLevel > 1000f)
        {
            EnergyLevel = 1000f;
        }

        UpdateEnergyVisual();
        TotalUsedEnergy -= amount;
    }

    public GameObject GetRandomTargetForEnemy()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue < 0.45f)
            return Striker;
        else
            return Defender;
    }
}

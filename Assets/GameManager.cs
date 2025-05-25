using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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

    public BasicEnemy[] BasicEnemyTypes;
    public BasicEnemy[] RageEnemyTypes;

    private StrikerPlayer StrikerPlayer;
    private DefenderPlayer DefenderPlayer;

    //private bool isTooClose;
    private float proximityWarningDuration = 3f;
    private float proximityTimer = 0f;

    [SerializeField]
    private GameObject gameOverScreen;

    public GameObject energyPickupPrefab;

    public bool didLevel1variant = false;

    public Slider bossBar;
    public TextMeshProUGUI bossBarText;

    [SerializeField]
    private GameObject endScreen;

    [SerializeField]
    private Image endImage;

    [SerializeField]
    private Sprite[] endImages;

    [SerializeField]
    private string[] endTexts;

    [SerializeField]
    private TextMeshProUGUI endTextDescription;

    [SerializeField]
    private TextMeshProUGUI endTextTitle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SaveLoad.Instance.LoadGame();
        SaveLoad.Instance.SaveGame();

        AudioListener.volume = PlayerPrefs.GetFloat("masterVolume", 100f);

        StrikerPlayer = Striker.GetComponent<StrikerPlayer>();
        DefenderPlayer = Defender.GetComponent<DefenderPlayer>();
    }

    public void DidLevel1Variant(bool setValue = false, bool value = false)
    {
        if(setValue == true)
            didLevel1variant = value;
        else
            didLevel1variant = true;
    }

    public void UpdateCurrentNavMesh()
    {
        var navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        navMeshSurface?.BuildNavMesh();
    }

    public void ShowFadeOutThenEndScreen()
    {
        StartCoroutine(ShowFadeOutThenEndScreenIE());
    }

    private IEnumerator ShowFadeOutThenEndScreenIE()
    {
        StartCoroutine(RoomManager.Instance.Fade(3, 2, 1));
        yield return new WaitForSeconds(3f);
        Time.timeScale = 0f;

        if (TotalUsedEnergy > 3500)
        {
            endImage.sprite = endImages[2];
            endTextDescription.text = endTexts[2];
            endTextTitle.text = "The End of the Line (3)";
        }
        else if(TotalUsedEnergy > 2000)
        {
            endImage.sprite = endImages[2];
            endTextDescription.text = endTexts[1];
            endTextTitle.text = "A Glimmer of Hope (2)";
        }
        else
        {
            endImage.sprite = endImages[0];
            endTextDescription.text = endTexts[0];
            endTextTitle.text = "A New Beginning (1)";
        }

        endScreen.SetActive(true);
        musicSource.Stop();

        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                Time.timeScale = 1f;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetEnergy(float value)
    {
        EnergyLevel = value;
        UpdateEnergyVisual();
    }

    public void YouDied(string playerName)
    {
        gameOverScreen.SetActive(true);
        gameOverScreen.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = $"{playerName} Died!";
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SaveLoad.Instance.LoadGame();
    }

    private void Update()
    {
        IncreaseEnergy(Time.deltaTime * 0.001f);

        if (Vector2.Distance(Striker.transform.position, Defender.transform.position) < 0.4f)
        {
            proximityTimer += Time.deltaTime;
            //isTooClose = true;

            if (proximityTimer >= proximityWarningDuration)
            {
                StrikerPlayer.TakeDamage(0.3f * Time.deltaTime);
                DefenderPlayer.TakeDamage(0.3f * Time.deltaTime);
                DecreaseEnergy(0.3f * Time.deltaTime);
                // visual/audio feedback here
            }
            else
            {
                // visual/audio warning
            }
        }
        else
        {
            proximityTimer = 0f;
            //isTooClose = false;
            // emove proximity warning visuals/audio
        }
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

    public GameObject GetRandomTargetForEnemy(float chanceForDefender = -1)
    {
        float randomValue = Random.Range(0f, 1f);
        if(chanceForDefender != -1)
            if (Random.Range(0f, 1) < chanceForDefender)
                return Defender;
            else
                return Striker;

        if(randomValue < 0.45f)
            return Striker;
        else 
            return Defender;
    }
}
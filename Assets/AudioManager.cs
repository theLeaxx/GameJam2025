using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float musicVolume;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip[] roomClips;

    // item 0 => boss 1, item 1 => boss 2, ...
    [SerializeField]
    private AudioClip[] bossClips;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVolume();
        SettingsManager.Instance.SaveOptions();
        StartCoroutine(PlayRandomSongExceptCurrentOne());
    }

    IEnumerator PlayRandomSongExceptCurrentOne()
    {
        var currentClip = musicSource.clip;

        if (currentClip == null)
        {
            currentClip = roomClips[Random.Range(0, roomClips.Length)];
            musicSource.clip = currentClip;
            musicSource.Play();
            yield return new WaitForSeconds(currentClip.length);
        }

        AudioClip newClip = null;
        while ((newClip == null || newClip == currentClip) && roomClips.Length > 1)
            newClip = roomClips[Random.Range(0, roomClips.Length)];

        musicSource.clip = newClip;
        musicSource.Play();
        UpdateVolume();
        yield return new WaitForSeconds(newClip.length);
        StartCoroutine(PlayRandomSongExceptCurrentOne());
    }

    public void BossTime()
    {
        StartCoroutine(BossTimeIE());
        StopCoroutine(PlayRandomSongExceptCurrentOne());
    }

    private IEnumerator BossTimeIE()
    {
        float fadeDuration = 1f;
        float startVolume = musicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        musicSource.clip = bossClips[0];
        musicSource.Play();
        musicSource.loop = true;
    }

    public void UpdateVolume()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100f) / 100f;
        musicSource.volume = musicVolume;
    }
}

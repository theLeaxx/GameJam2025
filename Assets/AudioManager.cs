using System.Collections;
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
        StartCoroutine(PlayRandomSongExceptCurrentOne());
    }

    // Update is called once per frame
    void Update()
    {
        
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
        while (newClip == null || newClip == currentClip)
            newClip = roomClips[Random.Range(0, roomClips.Length)];

        musicSource.clip = newClip;
        musicSource.Play();
        yield return new WaitForSeconds(newClip.length);
    }

    public void UpdateVolume()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100f) / 100f;
    }
}

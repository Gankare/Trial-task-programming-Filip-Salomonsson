using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [SerializeField] private AudioSource sfxSource;

    public AudioClip swordSwingClip;
    public AudioClip skeletonDeathClip;
    public AudioClip playerDyingClip;
    public AudioClip playerHurtClip;

    [Range(0.5f, 2f)] public float minPitch = 0.8f;
    [Range(0.5f, 2f)] public float maxPitch = 1.2f;

    private void Start()
    {
        if (sfxSource == null) 
            sfxSource = GetComponent<AudioSource>();
    }
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);

        sfxSource.pitch = 1f;
    }
    public void SwordSwingAudio()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        PlaySFX(swordSwingClip);
    }
    public void SkeletonDeathAudio()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        PlaySFX(skeletonDeathClip);
    }
    public void PlayerDyingAudio()
    {
        PlaySFX(playerDyingClip);
    }
    public void PlayerHurtAudio()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        PlaySFX(playerHurtClip);
    }
}

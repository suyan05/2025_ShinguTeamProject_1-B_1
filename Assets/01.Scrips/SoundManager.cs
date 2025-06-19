using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource effectSource;  // 효과음 재생용 AudioSource

    public AudioClip shootSFX;
    public AudioClip mergeSFX;
    public AudioClip destroySFX;
    public AudioClip attachSFX;
    // 필요하면 더 추가...

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }

        if (effectSource == null)
            effectSource = gameObject.AddComponent<AudioSource>();
    }

    // 효과음 재생 메서드들
    public void PlayShoot()
    {
        PlayEffect(shootSFX);
    }

    public void PlayMerge()
    {
        PlayEffect(mergeSFX);
    }

    public void PlayDestroy()
    {
        PlayEffect(destroySFX);
    }

    public void PlayAttach()
    {
        PlayEffect(attachSFX);
    }

    private void PlayEffect(AudioClip clip)
    {
        if (clip == null) return;
        effectSource.PlayOneShot(clip);
    }
}

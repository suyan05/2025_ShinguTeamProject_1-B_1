using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource effectSource;  // ȿ���� ����� AudioSource

    public AudioClip shootSFX;
    public AudioClip mergeSFX;
    public AudioClip destroySFX;
    public AudioClip attachSFX;
    // �ʿ��ϸ� �� �߰�...

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ���� ����
        }
        else
        {
            Destroy(gameObject);
        }

        if (effectSource == null)
            effectSource = gameObject.AddComponent<AudioSource>();
    }

    // ȿ���� ��� �޼����
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

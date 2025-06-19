using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource effectSource;  // ȿ���� ����� AudioSource

    public AudioClip shootSFX;
    public AudioClip mergeSFX;
    public AudioClip attachSFX;
    public AudioClip gameOverSFX;
    public AudioClip buttonClickSFX;
    public AudioClip PlayExplosionSFX;
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

    public void PlayAttach()
    {
        PlayEffect(attachSFX);
    }

    public void PlayGameOver()
    {
        PlayEffect(gameOverSFX);
    }

    public void PlayButtonClick()
    {
        PlayEffect(buttonClickSFX);
    }

    public void PlayExplosion()
    {
        PlayEffect(PlayExplosionSFX);
    }

    private void PlayEffect(AudioClip clip)
    {
        if (clip == null) return;
        effectSource.PlayOneShot(clip);
    }
}

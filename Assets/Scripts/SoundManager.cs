using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip swordSliceSound;
    [SerializeField] private AudioClip hitSound;

    private AudioSource source;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        source = GetComponent<AudioSource>();
    }

    public void PlayJump()     => source.PlayOneShot(jumpSound);
    public void PlayDash()     => source.PlayOneShot(dashSound);
    public void PlaySwordSlice() => source.PlayOneShot(swordSliceSound);
    public void PlayHit()      => source.PlayOneShot(hitSound);

    public void SetVolume(float value) => source.volume = value;
}
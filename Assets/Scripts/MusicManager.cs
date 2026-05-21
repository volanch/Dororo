using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip dialogueMusic;    // Level1
    [SerializeField] private AudioClip golemLevelMusic;  // Level2
    [SerializeField] private AudioClip bossFightMusic;   // Level3
    [SerializeField] private AudioClip gameOverMusic;

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
        source.loop = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu": Play(menuMusic);       break;
            case "Level1":   Play(dialogueMusic);   break;
            case "LevelGolem":   Play(golemLevelMusic); break;
            case "Level3":   Play(bossFightMusic);  break;
            case "GameOver": Play(gameOverMusic);   break;
        }
    }

    void Play(AudioClip clip)
    {
        if (clip == null || source.clip == clip) return;
        source.clip = clip;
        source.Play();
    }

    public void SetVolume(float value) => source.volume = value;

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
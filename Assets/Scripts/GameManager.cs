using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay")]
    public int servantsToKill = 3;
    private int servantsKilled = 0;
    private int score = 0;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI messageText; // "Kill the servants!"

    [Header("Boss")]
    [SerializeField] private GameObject bossObject; // drag boss here in Level2

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
        if (messageText != null)
            messageText.text = $"Servants: {servantsKilled}/{servantsToKill}";
    }

    public void AddScore(int amount)
    {
        score += amount;
        int high = PlayerPrefs.GetInt("HighScore", 0);
        if (score > high) PlayerPrefs.SetInt("HighScore", score);
        UpdateScoreUI();
    }

    public int GetScore() => score;
    public void ResetScore() => score = 0;

    public void UpdateHpUI()
    {
        var player = FindObjectOfType<Player>();
        if (hpText != null && player != null)
            hpText.text = "HP: " + player.currentHp;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void EnemyKilled()
    {
        servantsKilled++;
        if (messageText != null)
            messageText.text = $"Servants: {servantsKilled}/{servantsToKill}";

        if (servantsKilled >= servantsToKill)
        {
            // All servants dead — go to Level2 (boss level)
            Invoke(nameof(GoToBossLevel), 2f);
        }
    }

    void GoToBossLevel()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Win()
    {
        score += 100; // bonus
        PlayerPrefs.SetInt("HighScore", Mathf.Max(score, PlayerPrefs.GetInt("HighScore", 0)));
        SceneManager.LoadScene("GameOver");
    }
}
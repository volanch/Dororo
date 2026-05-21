using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathText;

    void Start()
    {
        if (DeathCounter.Instance != null)
            deathText.text = "Died: " + DeathCounter.Instance.deathCount;
        else
            deathText.text = "Died: 0";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Portrait")]
    [SerializeField] private Sprite playerPortrait;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private Coroutine typingCoroutine;
    private DialogueLine[] lines;

    void Start()
    {
        dialoguePanel.SetActive(false);

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level1")
        {
            StartDialogue(new DialogueLine[]
            {
                new DialogueLine("???",  "Agis! Give me back my soul. I know you took it.",    true),
                new DialogueLine("Agis", "Ah... so you found me. Impressive for a hollow shell.", false),
                new DialogueLine("Agis", "Yes, I took your soul. But I cannot return it.",       false),
                new DialogueLine("???",  "Cannot... or will not?",                               true),
                new DialogueLine("Agis", "My son. His soul was stolen long before yours.",       false),
                new DialogueLine("Agis", "He wanders as a shadow, just as you do now.",          false),
                new DialogueLine("Agis", "Find the one who took his soul. Bring it back to me.", false),
                new DialogueLine("Agis", "Do that... and I will return what is yours.",          false),
                new DialogueLine("???",  "And if I refuse?",                                     true),
                new DialogueLine("Agis", "Then you remain nothing. Forever.",                    false),
            });
        }
        // На LevelGolem диалог запустит скрипт голема через StartDialogue()
    }

    public void StartDialogue(DialogueLine[] newLines)
    {
        lines = newLines;
        currentLine = 0;
        dialoguePanel.SetActive(true);

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.dialoguePlaying = true;

        ShowLine(0);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                skipTyping = true;
            else
            {
                currentLine++;
                if (currentLine < lines.Length)
                    ShowLine(currentLine);
                else
                    EndDialogue();
            }
        }
    }

    void ShowLine(int index)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speakerText.text = lines[index].speaker;
        skipTyping = false;

        if (lines[index].isPlayerLine)
        {
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = playerPortrait;
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        typingCoroutine = StartCoroutine(TypeLine(lines[index].text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            if (skipTyping)
            {
                dialogueText.text = text;
                break;
            }
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.dialoguePlaying = false;

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "LevelGolem")
            StartCoroutine(TransitionToLevel("Level3"));
        else if (sceneName == "Level1")
            StartCoroutine(TransitionToLevel("LevelGolem"));
    }

    IEnumerator TransitionToLevel(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public bool isPlayerLine;

    public DialogueLine(string speaker, string text, bool isPlayerLine)
    {
        this.speaker = speaker;
        this.text = text;
        this.isPlayerLine = isPlayerLine;
    }
}
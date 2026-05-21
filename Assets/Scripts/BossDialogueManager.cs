using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossDialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;
    
    [Header("Portrait")]
    [SerializeField] private Sprite playerPortrait;

    [Header("Boss")]
    [SerializeField] private Boss boss;

    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private Coroutine typingCoroutine;
    private DialogueLine[] lines;

    void Start()
    {
        boss.enabled = false;

        lines = new DialogueLine[]
        {
            new DialogueLine("???",  "Agis! I have returned your son's soul. Now give me mine back.", true),
            new DialogueLine("Agis", "Heh... you actually did it. I am impressed, shadow.",           false),
            new DialogueLine("Agis", "But I am afraid I have no intention of keeping my word.",        false),
            new DialogueLine("???",  "You... you deceived me. This whole time.",                       true),
            new DialogueLine("Agis", "Your soul is too powerful to simply return.",                    false),
            new DialogueLine("Agis", "With it, I will become unstoppable.",                            false),
            new DialogueLine("???",  "Then I will take it back by force.",                             true),
            new DialogueLine("Agis", "You are nothing but a shadow. You cannot defeat me.",            false),
            new DialogueLine("???",  "Watch me.",                                                      true),
        };

        ShowLine(0);
    }

    void Update()
    {
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
        StartCoroutine(StartBossFight());
    }

    IEnumerator StartBossFight()
    {
        yield return new WaitForSeconds(0.5f);
        boss.enabled = true; // activate boss AI
        FindFirstObjectByType<Player>().dialoguePlaying = false; // unlock player
    }
}
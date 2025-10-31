using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI;
    public TMP_Text dialogueText;
    public Button[] optionButtons;
    public Button nextButton;

    [Header("Optional Cutscene Animator")]
    public Animator cutsceneAnimator;

    [Header("Dialogue Data")]
    public DialogueNode[] nodes;

    private int currentNodeIndex;
    private AutoTypewriterTMP typer;
    private bool isTyping = false;
    private bool inDialogue = false;

    public GameObject npc;

    public static DialogueManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        typer = dialogueText.GetComponent<AutoTypewriterTMP>();
        dialogueUI.SetActive(false);
    }

    public void StartDialogue(DialogueNode[] dialogueNodes)
    {

        if (inDialogue) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        nodes = dialogueNodes;
        currentNodeIndex = 0;
        inDialogue = true;
        dialogueUI.SetActive(true);
        DisplayNode(currentNodeIndex);
    }

    void DisplayNode(int index)
    {
        if (index < 0 || index >= nodes.Length)
        {
            EndDialogue();
            return;
        }

        currentNodeIndex = index;
        DialogueNode node = nodes[index];

        if (node.isCutscene)
        {
            dialogueUI.SetActive(false);
            if (cutsceneAnimator && !string.IsNullOrEmpty(node.cutsceneTrigger))
                cutsceneAnimator.SetTrigger(node.cutsceneTrigger);
            return;
        }

        dialogueText.text = node.dialogueText;
        typer.OnTypingComplete -= OnTypingFinished;
        typer.OnTypingComplete += OnTypingFinished;
        isTyping = true;

        foreach (Button btn in optionButtons) btn.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    void OnTypingFinished()
    {
        isTyping = false;
        DialogueNode node = nodes[currentNodeIndex];

        if (node.useNextButton)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.RemoveAllListeners();

            int nextNode = FindNextNodeWithOptions(currentNodeIndex);

            if (nextNode >= 0)
                nextButton.onClick.AddListener(() => DisplayNode(nextNode));
            else
                nextButton.onClick.AddListener(EndDialogue);
        }
        else
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < node.options.Length)
                {
                    optionButtons[i].gameObject.SetActive(true);
                    optionButtons[i].GetComponentInChildren<TMP_Text>().text = node.options[i].text;

                    int next = node.options[i].nextNodeID;
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => DisplayNode(next));
                }
                else
                    optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    int FindNextNodeWithOptions(int startIndex)
    {
        for (int i = startIndex + 1; i < nodes.Length; i++)
        {
            if (nodes[i].options != null && nodes[i].options.Length > 0)
            {
                return i; 
            }
        }
        return -1; 
    }

    public void EndDialogue()
    {
        inDialogue = false;
        dialogueUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EnemySpawner.Instance.SpawnObjectsOutsideCamera();
        npc.SetActive(false);
    }

    public bool IsInDialogue() => inDialogue;
}

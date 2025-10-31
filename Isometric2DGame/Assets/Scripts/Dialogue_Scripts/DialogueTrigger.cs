using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueNode[] dialogue;
    private bool playerInRange = false;
    public GameObject interactText;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.IsInDialogue())
            {
                interactText.SetActive(false);
                DialogueManager.Instance.StartDialogue(dialogue);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}

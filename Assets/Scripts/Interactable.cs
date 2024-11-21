using TMPro;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    [SerializeField]
    protected TMP_Text NPCText;
    protected DialogueManager dialogueManager;
    public string prompt;
    public DialogueText dialogue;
    public bool dialogueEnd = false;

    protected virtual void Start() {
        dialogueManager = GameObject.Find("/DialogueManager").GetComponent<DialogueManager>();
    }

    protected virtual void Update() {
        if (dialogueEnd) {
            dialogueEnd = false;
            OnDialogueEnd();
        }
    }

    public abstract void Interact();

    public virtual void OnDialogueEnd() {
        return;
    }
}

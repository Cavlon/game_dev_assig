using TMPro;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    [SerializeField]
    protected TMP_Text NPCText;
    protected DialogueManager dialogueManager;
    public string prompt;

    [SerializeField]
    protected DialogueText dialogue;

    protected virtual void Start() {
        dialogueManager = GameObject.Find("/DialogueManager").GetComponent<DialogueManager>();
    }

    public abstract void Interact();
}

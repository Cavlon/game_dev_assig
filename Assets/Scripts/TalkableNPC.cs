using UnityEngine;

public class TalkableNPC : Interactable
{
    public override void Interact()
    {
        dialogueManager.StartDialogue(NPCText, dialogue, null);
    }
}

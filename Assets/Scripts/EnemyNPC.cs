using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyNPC : Interactable
{
    [SerializeField]
    private GameObject opponentPrefab;

    public override void Interact()
    {
        StaticData.nextOpponent = opponentPrefab;
        dialogueManager.StartDialogue(NPCText, dialogue, this);
    }

    public override void OnDialogueEnd()
    {
        SceneManager.LoadScene("Combat");
    }
}

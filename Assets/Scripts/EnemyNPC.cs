using UnityEngine;

public class EnemyNPC : Interactable
{
    [SerializeField]
    private GameObject opponentPrefab;
    [SerializeField]
    private DialogueText winDialogue;
    [SerializeField]
    private DialogueText loseDialogue;

    public int bossVal;

    public override void Interact()
    {
        dialogueManager.StartDialogue(NPCText, dialogue, StartBattle);
    }

    protected override void Start() {
        base.Start();
        if (StaticData.enemyVal == bossVal) {
            BatteOver();
        }
    }

    public void StartBattle()
    {
        StaticData.playerPos = GameObject.Find("/Player").transform.position;
        StaticData.nextOpponent = opponentPrefab;
        StaticData.enemyVal = bossVal;
        StartCoroutine(GameObject.Find("/SceneLoader").GetComponent<SceneLoader>().ChangeScene("Combat"));
    }

    private void BatteOver() {
        if (StaticData.battleWon) {
            if (bossVal > StaticData.bossesBeat) StaticData.bossesBeat = bossVal;
            dialogueManager.StartDialogue(NPCText, winDialogue, BossBeat);
        } else {
            dialogueManager.StartDialogue(NPCText, loseDialogue, null);
        }
    }

    public void BossBeat() {
        GameObject.Find("/GameManager").GetComponent<OverworldManager>().BossJustBeat();
    }
    
}

using System.Collections;
using TMPro;
using CavlonUtils;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private float textScrollSpeed = 0.5f;
    [SerializeField]
    private Player player;

    [SerializeField]
    private TMP_Text playerText;
    private Transform playerTextBox;

    private TMP_Text NPCText;
    private Transform NPCTextBox;
    private Interactable interactable;
    private DialogueText currentDialogue;

    public int playerInd = 0;
    public int NPCInd = 0;
    public int turn = 0;
    private bool prevPlayerTalk = false;
    private bool canAdvance = false;

    void Start() {
        playerTextBox = playerText.transform.parent;
    }

    void Update() {
        if (canAdvance && Input.GetKeyDown(KeyCode.Space)) {
            canAdvance = false;
            if (turn < currentDialogue.speakTurnPlayer.Length) {
                if (currentDialogue.speakTurnPlayer[turn]) {
                    StartCoroutine(DisplayPlayerText());
                } else {
                    StartCoroutine(DisplayNPCText());
                }
            } else {
                Debug.Log("Ending Dialogue");
                interactable.dialogueEnd = true;
                if (prevPlayerTalk) {
                    StartCoroutine(CloseTextBox(playerTextBox, true));
                } else {
                    StartCoroutine(CloseTextBox(NPCTextBox, true));
                }
            }
        }
    }

    public void StartDialogue(TMP_Text _NPCText, DialogueText dialogue, Interactable _interactable) {
        Debug.Log("Starting Dialogue");
        NPCText = _NPCText;
        NPCTextBox = _NPCText.transform.parent;
        interactable = _interactable;
        currentDialogue = dialogue;

        turn = 0;
        playerInd = 0;
        NPCInd = 0;
        player.canInteract = false;
        player.canMove = false;

        if (currentDialogue.speakTurnPlayer[turn]) {
            StartCoroutine(DisplayPlayerText());
        } else {
            StartCoroutine(DisplayNPCText());
        }
    }

    public IEnumerator DisplayPlayerText() {
        int newLineInd = 0;

        playerText.text = string.Empty;

        if (turn == 0) {
            playerTextBox.gameObject.SetActive(true);
            yield return AnimUtils.TweenScale(playerTextBox, Vector2.one, 0.4f, AnimUtils.CubicOut);
        } else if (!prevPlayerTalk) {
            StartCoroutine(CloseTextBox(NPCTextBox));
            playerTextBox.gameObject.SetActive(true);
            yield return AnimUtils.TweenScale(playerTextBox, Vector2.one, 0.4f, AnimUtils.CubicOut);
        }

        for (int i = 0; i < currentDialogue.playerLines[playerInd].Length; i++) {
            if (newLineInd > 20 && currentDialogue.playerLines[playerInd][i] == ' ') {
                playerText.text += "<br>";
                newLineInd = 0;
                yield return new WaitForSeconds(textScrollSpeed);
                continue;
            }
            playerText.text += currentDialogue.playerLines[playerInd][i];
            newLineInd++;
            yield return new WaitForSeconds(textScrollSpeed);
        }
        playerInd++;
        turn++;

        canAdvance = true;
        prevPlayerTalk = true;
    }

    public IEnumerator DisplayNPCText() {
        int newLineInd = 0;

        NPCText.text = string.Empty;

        if (turn == 0) {
            NPCTextBox.gameObject.SetActive(true);
            yield return AnimUtils.TweenScale(NPCTextBox, Vector2.one, 0.4f, AnimUtils.CubicOut);
        } else if (prevPlayerTalk) {
            StartCoroutine(CloseTextBox(playerTextBox));
            NPCTextBox.gameObject.SetActive(true);
            yield return AnimUtils.TweenScale(NPCTextBox, Vector2.one, 0.4f, AnimUtils.CubicOut);
        }

        for (int i = 0; i < currentDialogue.NPCLines[NPCInd].Length; i++) {
            if (newLineInd > 20 && currentDialogue.NPCLines[NPCInd][i] == ' ') {
                NPCText.text += "<br>";
                newLineInd = 0;
                yield return new WaitForSeconds(textScrollSpeed);
                continue;
            }
            NPCText.text += currentDialogue.NPCLines[NPCInd][i];
            newLineInd++;
            yield return new WaitForSeconds(textScrollSpeed);
        }
        NPCInd++;
        turn++;

        canAdvance = true;
        prevPlayerTalk = false;
    }

    private IEnumerator CloseTextBox(Transform targetTextBox, bool endDialogue = false) {
        yield return AnimUtils.TweenScale(targetTextBox, new Vector2(0.01f, 0.01f), 0.4f, AnimUtils.CubicIn);
        targetTextBox.gameObject.SetActive(false);
        player.canInteract = endDialogue;
        player.canMove = endDialogue;
    }

}

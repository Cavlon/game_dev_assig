using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueText")]
public class DialogueText : ScriptableObject
{
    public string[] playerLines = new string[0];
    public string[] NPCLines = new string[0];
    public bool[] speakTurnPlayer = new bool[0];
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Opponent : MonoBehaviour
{
    public SlotManager slotManager;
    public GameManager gameManager;
    public int credits;

    [SerializeField]
    protected CardData[] cards;

    public abstract IEnumerator OpponentTurn();
}

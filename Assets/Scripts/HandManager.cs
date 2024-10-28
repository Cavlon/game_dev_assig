using UnityEngine;
using Global;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;

public class HandManager : MonoBehaviour
{

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Transform handTrans;

    private float spread = 6f;
    private float spacing = 90f;
    private float vertOffset = 20f;
    private List<GameObject> cards = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(Card cardData) {
        GameObject newCard = Instantiate(cardPrefab, handTrans.position, Quaternion.identity, handTrans);
        cards.Add(newCard);

        newCard.GetComponent<UpdateCard>().cardData = cardData;

        UpdateHand();
    }

    private void UpdateHand()
    {
        int cardCount = cards.Count;
        float midpoint = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++) {
            float centralDist = (i - midpoint);
            cards[i].transform.localRotation = Quaternion.Euler(0, 0, spread * centralDist);

            cards[i].transform.localPosition = new Vector3(centralDist * -spacing, Math.Abs(centralDist) * -vertOffset, 0);
        }
    }
}

using System.Collections;
using UnityEngine;
using CavlonUtils;

public class ShopNPC : Interactable
{

    private Transform shopUI;
    private Shop shop;
    private IEnumerator shopOpenEnumerator;

    [SerializeField]
    private DialogueText openShopDialogue;
    private bool introduced = false;
    private bool shopOpen = false;

    private OverworldManager overworldManager;

    protected override void Start() {
        base.Start();
        shopUI = transform.GetChild(2).GetChild(0);
        shop = shopUI.GetComponent<Shop>();
        shopUI.gameObject.SetActive(false);
        overworldManager = GameObject.Find("/GameManager").GetComponent<OverworldManager>();
    }

    protected override void Update() {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape) && shopOpen) {
            CloseShop();
        }
    }

    public override void Interact()
    {
        if (!introduced) {
            dialogueManager.StartDialogue(NPCText, dialogue, this);
        } else {
            dialogueManager.StartDialogue(NPCText, openShopDialogue, this);
        }
    }

    public override void OnDialogueEnd()
    {
        if (!introduced) {
            introduced = true;
            return;
        }
        Debug.Log("Opening Shop");
        overworldManager.canPause = false;
        shopOpen = true;
        shop.cardsDrawn = false;
        shopUI.gameObject.SetActive(true);
        shop.UpdateVisuals();
        if (shopOpenEnumerator != null) {
            StopCoroutine(shopOpenEnumerator);
        }
        shopOpenEnumerator = AnimUtils.TweenPos(shopUI, Vector2.zero, 1.5f, AnimUtils.QuintOut);
        StartCoroutine(shopOpenEnumerator);
    }

    public void CloseShop() {
        shop.DestroyCards();
        StartCoroutine(CloseShopEnumerator());
    }

    private IEnumerator CloseShopEnumerator() {
        if (shopOpenEnumerator != null) {
            StopCoroutine(shopOpenEnumerator);
        }
        shopOpenEnumerator = AnimUtils.TweenPos(shopUI, new Vector2(0, 1121), 1f, AnimUtils.QuintOut);
        yield return shopOpenEnumerator;
        shopUI.gameObject.SetActive(false);
        overworldManager.canPause = true;
        shopOpen = false;
    }
}

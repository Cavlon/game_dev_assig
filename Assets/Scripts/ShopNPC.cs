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
    [SerializeField]
    private DialogueText closeShopDialogue;
    private bool shopOpen = false;

    private OverworldManager overworldManager;
    private SoundManager soundManager;
    private Player player;

    protected override void Start() {
        base.Start();
        shopUI = transform.GetChild(2).GetChild(0);
        shop = shopUI.GetComponent<Shop>();
        shopUI.gameObject.SetActive(false);
        overworldManager = GameObject.Find("/GameManager").GetComponent<OverworldManager>();
        soundManager = GameObject.Find("/SoundManager").GetComponent<SoundManager>();
        player = GameObject.Find("/Player").GetComponent<Player>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && shopOpen) {
            CloseShop();
        }
    }

    public override void Interact()
    {
        if (!StaticData.shopIntroduced) {
            dialogueManager.StartDialogue(NPCText, dialogue, EndIntroduction);
        } else {
            dialogueManager.StartDialogue(NPCText, openShopDialogue, OpenShop);
        }
    }

    public void EndIntroduction() {
        StaticData.shopIntroduced = true;
    }

    public void OpenShop()
    {
        Debug.Log("Opening Shop");
        overworldManager.canPause = false;
        shopOpen = true;
        shop.cardsDrawn = false;
        shopUI.gameObject.SetActive(true);
        shop.UpdateVisuals();
        soundManager.PlaySound(2);

        player.canInteract = false;
        player.canControl = false;

        if (shopOpenEnumerator != null) {
            StopCoroutine(shopOpenEnumerator);
        }
        shopOpenEnumerator = AnimUtils.TweenPos(shopUI, Vector2.zero, 1.5f, AnimUtils.QuintOut);
        StartCoroutine(shopOpenEnumerator);
    }

    public void CloseShop() {
        if (!shop.canClick) return;
        dialogueManager.StartDialogue(NPCText, closeShopDialogue, null);
        soundManager.PlaySound(2);
        shop.DestroyCards();
        StartCoroutine(CloseShopEnumerator());
    }

    private IEnumerator CloseShopEnumerator() {
        shop.canClick = false;
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

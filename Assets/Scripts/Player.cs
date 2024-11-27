using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CavlonUtils;
using TMPro;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float gravity = 9.8f;
    [SerializeField]
    private float interactRadius = 5f;
    [SerializeField]
    private LayerMask interactLayerMask;

    private readonly Collider[] interactionColliders = new Collider[1];

    private Animator spriteAnimator;
    private SpriteRenderer sprite;

    private CharacterController characterController;
    private State state = State.Idle;
    private int dir = 1;
    public bool canControl = true;
    private bool canMove = false;

    public bool canInteract = true;
    private Interactable interactTarget = null;
    private TMP_Text promptText;
    private IEnumerator interactPromptEnumerator = null;
    private bool hiding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        spriteAnimator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        promptText = GetComponentInChildren<TMP_Text>();
        promptText.gameObject.SetActive(false);
        transform.position = StaticData.playerPos;
        StartCoroutine(DelayControl());
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractables();
        if (canMove) {
            PlayerMove();
        }        
    }

    private void CheckInteractables() {
        if (canInteract) {

            int interactablesFound = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, interactionColliders, interactLayerMask);

            if (interactTarget == null) {
                if (interactablesFound > 0) {
                    interactTarget = interactionColliders[0].GetComponent<Interactable>();
                    ShowPrompt();
                }
            } else {
                if (interactablesFound == 0) {
                    interactTarget = null;
                    StartCoroutine(HidePrompt());
                } else {
                    ShowPrompt();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && !OverworldManager.paused && interactTarget != null) {
                interactTarget.Interact();
            }

        } else {
            if (promptText.gameObject.activeSelf && !hiding) {
                StartCoroutine(HidePrompt());
            }
        }
    }

    private void PlayerMove() {
        Vector3 velocity = Vector3.zero;
        if (canControl) {
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        if (dir * velocity.x < 0) {
            sprite.flipX = !sprite.flipX;
            dir *= -1;
        }

        float mag = velocity.magnitude;

        if (mag > 0 && state != State.Run) {
            spriteAnimator.Play("PlayerRun");
            state = State.Run;
        } else if (mag == 0 && state != State.Idle) {
            spriteAnimator.Play("PlayerIdle");
            state = State.Idle;
        }

        if (mag > 1) {
            velocity = velocity / mag;
        }

        velocity.y = -gravity;

        characterController.Move(velocity * Time.deltaTime * speed);  
    }

    private void ShowPrompt() {
        if (!promptText.gameObject.activeSelf) {
            promptText.text = "<sprite=0> " + interactTarget.prompt;

            promptText.gameObject.SetActive(true);
            if (interactPromptEnumerator != null) {
                StopCoroutine(interactPromptEnumerator);
            }
            interactPromptEnumerator = AnimUtils.TweenScale(promptText.transform, Vector2.one, 0.3f, AnimUtils.CubicIn);
            StartCoroutine(interactPromptEnumerator);
        }
    }

    private IEnumerator HidePrompt() {
        hiding = true;
        if (interactPromptEnumerator != null) {
            StopCoroutine(interactPromptEnumerator);
        }
        interactPromptEnumerator = AnimUtils.TweenScale(promptText.transform, new Vector2(0.01f, 0.01f), 0.3f, AnimUtils.CubicIn);
        yield return interactPromptEnumerator;
        promptText.gameObject.SetActive(false);
        hiding = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    private IEnumerator DelayControl() {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    private enum State {
        Idle,
        Run
    }
}

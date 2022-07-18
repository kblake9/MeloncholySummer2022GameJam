using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Meloncholy m_pia;
    private Rigidbody2D m_rb;
    private Animator m_animator;
    private CapsuleCollider2D m_cac;
    private CircleCollider2D m_cic;

    private float x = 0;
    [Header("Mel Modifiers")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 18f;
    [SerializeField] private float slashFloat = 13;
    [SerializeField] private float slashPower = 5;
    [SerializeField] PlayerState playerState = PlayerState.None;

    private enum PlayerState { None, Denial, Anger, Bargaining, Depression, Acceptance }

    [Header("Mel Cooldowns")]
    [SerializeField] private bool hasRollCooldown;
    [SerializeField] private float slashCooldownTime = .7f;
    [SerializeField] private float inflictRollCooldownTime = 1f;

    [Header("Prefab Starters")]
    [SerializeField] private GameObject m_inputIndicator;

    private bool canMove = true;
    private bool crouching = false;

    private int lastX = 1;

    public Meloncholy PIA => m_pia;

    #region Singleton
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
            }

            return instance;
        }
    }
    #endregion
    private bool canSlash = true;
    private bool canInflictRollForce = true;

    private void Awake()
    {
        m_pia = new Meloncholy();
    }

    void Start()
    {
        //Intializing Components
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_cac = GetComponent<CapsuleCollider2D>();
        m_cic = GetComponent<CircleCollider2D>();

        //Component Default Enablings
        

        //Inserting Action Methods to bindings
        m_pia.Player.Jump.started += Jump;
        m_pia.Player.Paint.started += PaintAction;
        m_pia.External.DialogueContinue.started += DialogueContinueAction;

        //Enabling Actions
        m_pia.Player.Movement.Enable();
        m_pia.Player.Jump.Enable();
        m_pia.Player.Paint.Enable();
        m_pia.Player.Interact.Enable();      
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        x = m_pia.Player.Movement.ReadValue<Vector2>().x;
        m_animator.SetFloat("CurrX", x);
        if (Mathf.Abs(x) > 0.2f)
        {
            m_animator.SetFloat("LastX", x);
            lastX = x > 0 ? 1 : -1;
        }
        if (!crouching && canSlash)
        {
            if (m_pia.Player.Movement.ReadValue<Vector2>().y < -0.5)
            {
                if (checkGround() && canMove)
                {
                    Crouch(true);
                }
            }
        }
        else if (m_pia.Player.Movement.ReadValue<Vector2>().y >= 0
            && checkStandRoom() && canMove && (m_animator.GetCurrentAnimatorStateInfo(0).IsName("RollingLeft") || m_animator.GetCurrentAnimatorStateInfo(0).IsName("RollingRight")))
        {
            Crouch(false);
        }
        if (canMove && !crouching && Mathf.Abs(x) > 0.2f)
        {
            transform.Translate(Vector2.right * x * Time.deltaTime * moveSpeed);
        }
        if(!crouching && Mathf.Abs(m_rb.velocity.x) > 1)
        {
            m_rb.velocity = m_rb.velocity = new Vector2(m_rb.velocity.x/Mathf.Abs(m_rb.velocity.x), m_rb.velocity.y);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (checkGround())
        {
            m_animator.ResetTrigger("Jump");
            m_animator.SetTrigger("Jump");
            m_rb.velocity *= Vector2.right;
            m_rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    private void Crouch(bool shouldCrouch)
    {
        m_animator.SetBool("Roll", shouldCrouch);
        crouching = shouldCrouch;
        m_cac.enabled = !shouldCrouch;
        m_cic.enabled = shouldCrouch;
        m_rb.freezeRotation = !shouldCrouch;
        m_rb.velocity *= 0f;
        transform.eulerAngles = shouldCrouch ? transform.eulerAngles : Vector3.zero;
        canSlash = !shouldCrouch;     
    }

    private void PaintAction(InputAction.CallbackContext context)
    {
        if (canSlash)
        {
            m_animator.ResetTrigger("Slash");
            m_animator.SetTrigger("Slash");
            if (!checkGround())
            {
                m_rb.velocity *= (m_rb.velocity.y > 0) ? new Vector2(1, .5f) : new Vector2(1, 0);
                m_rb.AddForce(Vector2.up * slashFloat, ForceMode2D.Impulse);
            }

            RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y), new Vector2(.1f, 1.2f), 0.0f, Vector2.right * lastX, 1.5f);
            if (hits != null)
            {
                for (int i = 0; i < hits.Length; ++i)
                {
                    if (playerState == PlayerState.Anger)
                    {
                        Rigidbody2D target_rb = hits[i].collider.gameObject.GetComponent<Rigidbody2D>();
                        if (target_rb != null && hits[i].collider.gameObject.tag.Equals("Physics Objects"))
                        {
                            Vector3 dir = hits[i].collider.gameObject.transform.position - transform.position;
                            target_rb.AddForce(slashPower * new Vector2(dir.x, dir.y + 1), ForceMode2D.Impulse);
                        }
                    }
                }
            }

            StartCoroutine(slashCoolDown());
        }
        else if (crouching && canInflictRollForce)
        {
            //Allows Mel to inflict force when rolling
            if (Mathf.Abs(m_rb.velocity.x) <= 18f)
            {
                m_rb.AddForce((lastX >= 0 ? Vector2.right : Vector2.left) * moveSpeed,
                ForceMode2D.Impulse);

                m_rb.AddTorque(-1f * (lastX * moveSpeed) / 2.5f,
                ForceMode2D.Impulse);
            }
            StartCoroutine(InflictRollForceCoolDown());   
        }
        
    }

    private void DialogueContinueAction(InputAction.CallbackContext context)
    {
        DialogueManager.Instance.ClickBehavior();
    }


    private bool rollingDownward = false;
    [HideInInspector] public Transform cliffBotPlat;

    private void RollDownCliff(InputAction.CallbackContext context)
    {
        if (!rollingDownward)
        {
            rollingDownward = true;
            float y = cliffBotPlat.position.y;
            m_pia.Player.Movement.Disable();
            StartCoroutine(RollDownward(y));
        } 
    }

    private IEnumerator slashCoolDown ()
    {
        canSlash = false;
        yield return new WaitForSeconds(slashCooldownTime);
        canSlash = true;
    }

    private IEnumerator InflictRollForceCoolDown()
    {
        canInflictRollForce = false;
        if (hasRollCooldown)
        {
            yield return new WaitForSeconds(inflictRollCooldownTime);
        }
        else
        {
            yield return new WaitUntil(() => !crouching);
        }
        
        canInflictRollForce = true;
    }

    private IEnumerator RollDownward(float target)
    {    
        while (transform.position.y > target + 3f)
        {
            transform.Translate(Vector2.down * Time.deltaTime * 2);
            yield return new WaitForSeconds(0.2f);
        }

        rollingDownward = false;
        m_pia.Player.Movement.Enable();
    }

    private bool checkGround ()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y-(.9f / (crouching ? 3 : 1))),new Vector2(.7f,.1f),0,Vector2.down,.1f);
        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.gameObject == this.gameObject)
                {
                    
                }
                else if (hits[i].collider.isTrigger)
                {

                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private bool checkStandRoom ()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y + .4f), new Vector2(.45f, .1f), 0, Vector2.up, 1.2f);
        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.gameObject == this.gameObject)
                {

                }
                else if (hits[i].collider.isTrigger)
                {

                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Interactable>().requireInput)
        {
            m_inputIndicator.SetActive(true);
            m_pia.Player.Paint.Disable();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Interactable npc = collision.GetComponent<Interactable>();
        if (npc != null && npc.isNPC && m_pia.Player.Interact.IsPressed())
        {
            collision.GetComponent<Interactable>().OnInteract();           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Interactable>().requireInput)
        {
            m_inputIndicator.SetActive(false);
            m_pia.Player.Paint.Enable();
        }
    }
}

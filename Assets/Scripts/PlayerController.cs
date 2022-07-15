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
    
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private float slashFloat = 10;

    private enum PlayerState {None, Denial, Anger, Bargaining, Depression, Acceptance}
    [SerializeField] PlayerState playerState = PlayerState.None;

    [SerializeField] private float slashCooldownTime = .5f;
    [SerializeField] private float slashPower = 1;

    private bool canMove = true;
    private bool crouching = false;

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

        //Inserting Action Methods to bindings
        m_pia.Player.Jump.started += Jump;
        m_pia.Player.Paint.started += PaintAction;
        m_pia.Player.RollDownCliff.started += RollDownCliff;

        //Enabling Actions
        m_pia.Player.Movement.Enable();
        m_pia.Player.Jump.Enable();
        m_pia.Player.Paint.Enable();    
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        x = m_pia.Player.Movement.ReadValue<Vector2>().x;
        m_animator.SetFloat("CurrX", x);
        if (x > 0.2 || x < -0.2)
        {
            m_animator.SetFloat("LastX", x);
        }

        //if (playerState == PlayerState.Depression)
        //{
            if (!crouching)
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
                && checkStandRoom() && canMove)
            {
                Crouch(false);
            }

        //}
        if (canMove && !crouching)
        {
            transform.Translate(Vector2.right * x * Time.deltaTime * moveSpeed);
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
            if (playerState == PlayerState.Anger)
            {
                RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y), new Vector2(.1f, 1.2f), 0.0f, Vector2.right, 1.5f);
                if (hits != null)
                {
                    for (int i = 0; i < hits.Length; ++i)
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
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y-.9f),new Vector2(.8f,.1f),0,Vector2.down,.1f);
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
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y + .4f), new Vector2(.9f, .1f), 0, Vector2.up, 1.2f);
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
}

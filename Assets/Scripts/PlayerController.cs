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
        if (playerState == PlayerState.Depression)
        {
            if (!crouching)
            {
                if (m_pia.Player.Movement.ReadValue<Vector2>().y < 0)
                {
                    if (checkGround() && canMove)
                    {
                        Crouch(true);
                    }
                }
            }
            else
            {
                if (m_pia.Player.Movement.ReadValue<Vector2>().y >= 0)
                {
                    if (checkStandRoom() && canMove)
                    {
                        Crouch(false);
                    }
                }
            }
        }
        if (canMove && !crouching)
        {
            transform.Translate(Vector2.right * x * Time.deltaTime * moveSpeed);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (checkGround())
        {
            m_rb.velocity *= Vector2.right;
            m_rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    private void Crouch(bool shouldCrouch)
    {
        crouching = shouldCrouch;
        m_cac.enabled = !shouldCrouch;
        m_cic.enabled = shouldCrouch;
        m_rb.freezeRotation = !shouldCrouch;
        m_rb.velocity *= .6f;
        transform.eulerAngles = shouldCrouch ? transform.eulerAngles : Vector3.zero;
        canSlash = !shouldCrouch;
    }

    private void PaintAction(InputAction.CallbackContext context)
    {
        if (canSlash)
        {
            if (!checkGround())
            {
                m_rb.velocity *= (m_rb.velocity.y > 0) ? new Vector2(1, .5f) : new Vector2(1, 0);
                m_rb.AddForce(Vector2.up * slashFloat, ForceMode2D.Impulse);
            }
            StartCoroutine(slashCoolDown());
        }
    }

    private bool rollingDownward = false;
    public Transform cliffBotPlat;

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
                if (hits[i].collider.gameObject != this.gameObject)
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
                if (hits[i].collider.gameObject != this.gameObject)
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

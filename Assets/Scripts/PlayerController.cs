using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Meloncholy m_pia;
    private Rigidbody2D m_rb;
    private float x = 0;
    
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private float slashFloat = 5;

    private bool canMove = true;

    public Meloncholy PIA => m_pia;

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
    private bool canSlash = true;
    

    private void Awake()
    {
        m_pia = new Meloncholy();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_pia.Player.Jump.started += Jump;
        m_pia.Player.Paint.started += PaintAction;
        m_pia.Player.Enable();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        x = m_pia.Player.Movement.ReadValue<Vector2>().x;
        if (canMove)
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

    IEnumerator slashCoolDown ()
    {
        canSlash = false;
        yield return new WaitForSeconds(.5f);
        canSlash = true;
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
}

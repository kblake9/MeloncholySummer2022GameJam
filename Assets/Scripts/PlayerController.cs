using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Meloncholy m_pia;
    private Rigidbody2D m_rb;
    private float x = 0;
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    private bool canMove = true;


    private void Awake()
    {
        m_pia = new Meloncholy();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_pia.Player.Jump.started += Jump;
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
            transform.Translate(Vector2.right * x * Time.deltaTime * speed);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (checkGround())
        {
            m_rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
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

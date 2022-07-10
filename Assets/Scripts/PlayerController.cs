using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Meloncholy m_pia;
    private Rigidbody2D m_rb;
    
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
        float speed = 2f;
        float x = m_pia.Player.Movement.ReadValue<Vector2>().x;
        transform.Translate(new Vector2(x * Time.deltaTime * speed, 0));

    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        m_rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    }


}

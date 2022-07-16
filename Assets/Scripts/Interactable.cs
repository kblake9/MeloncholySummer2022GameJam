using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
//Make sure to attach a collider to this script
public class Interactable : MonoBehaviour
{
    [SerializeField] public bool isNPC = false;
    [SerializeField] private UnityEvent m_onInteract;
    [SerializeField] private bool requireInput;

    public void OnInteract()
    {
        m_onInteract.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && !requireInput)
        {
            OnInteract();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Make sure to attach a collider to this script
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent m_onInteract;
    [SerializeField] public bool isNPC = false;

    public void OnInteract()
    {
        m_onInteract.Invoke();
    }
}

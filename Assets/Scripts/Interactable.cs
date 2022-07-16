using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Make sure to attach a collider to this script
public class Interactable : MonoBehaviour
{
    [SerializeField] public bool isNPC = false;
    [SerializeField] private UnityEvent m_onInteract; 

    public void OnInteract()
    {
        m_onInteract.Invoke();
    }
}

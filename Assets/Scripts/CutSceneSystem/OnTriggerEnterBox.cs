using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterBox : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogueTrigger;

    void OnTriggerEnter2D(Collider2D coll){
        dialogueTrigger.TriggerDialogue();
    }
}

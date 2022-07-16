using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterBoxCutScene : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogueTrigger;
    private bool firstTime = true;

    void OnTriggerEnter2D(Collider2D coll){
        if (firstTime) 
        {
            dialogueTrigger.TriggerDialogue();
            firstTime = false;
        }
    }
}

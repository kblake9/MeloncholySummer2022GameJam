using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public bool isColliding;
    private GameObject gameObject;
    public Dialogue[] dialogues;


    void Start()
    {
        isColliding = true;
    }
    

    public void TriggerDialogue()
    {
        if (isColliding) {
            Debug.Log("How many times is this being called?");
            DialogueManager.Instance.QueueDialogue(dialogues);
            isColliding = false;
        }
    }
}

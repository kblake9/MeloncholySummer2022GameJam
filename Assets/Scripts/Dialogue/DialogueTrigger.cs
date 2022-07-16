using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    private GameObject gameObject;
    public Dialogue[] dialogues;
    public bool isEndScene;
    [SerializeField] private string SceneName;



    

    public void TriggerDialogue()
    {
        DialogueManager.Instance.QueueDialogue(dialogues, SceneName, isEndScene);
    }
}

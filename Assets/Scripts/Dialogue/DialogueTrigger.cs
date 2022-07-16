using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public bool isColliding;
    private GameObject gameObject;
    public Dialogue[] dialogues;
    public bool isEndScene;
    [SerializeField] private string SceneName;

    void Start()
    {
        isColliding = true;
    }
    

    public void TriggerDialogue()
    {
        if (isColliding) {
            DialogueManager.Instance.QueueDialogue(dialogues, SceneName, isEndScene);
            isColliding = false;
        }
    }
}

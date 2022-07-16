using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicked : MonoBehaviour
{

    DialogueTrigger dialogueTrigger;
    // Start is called before the first frame update
    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    public void onButtonClick() {
        dialogueTrigger.TriggerDialogue();
    }
}

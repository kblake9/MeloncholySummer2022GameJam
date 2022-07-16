using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cutscene : MonoBehaviour
{
    public Animator animator;
    public Dialogue dialogue;
    [SerializeField] private PlayerController player;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private bool isCutScene;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    void StartCutScene()
    {
       
    }
}

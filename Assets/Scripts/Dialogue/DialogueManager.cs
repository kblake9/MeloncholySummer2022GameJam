using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{   
    private static DialogueManager instance;
    public static DialogueManager Instance 
    { 
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueManager>();
            }

            return instance;
        } 
    }
    private int peopleChange;
    private int count;

    [SerializeField]private Button continueButton;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    private string sceneName;

    public Animator animator;

    public Queue<string> sentences;
    private Queue<Dialogue> dialogueSet;
    private bool isEndScene;
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        dialogueSet = new Queue<Dialogue>();
        count = 0;
    }
    public void QueueDialogue(Dialogue[] dialogues, string SceneName, bool IsEndScene) {
        foreach (Dialogue dialogue in dialogues)
        {
            dialogueSet.Enqueue(dialogue);
        }
        isEndScene = IsEndScene;
        sceneName = SceneName;
        GetDialogue(dialogueSet);
        PlayerController.Instance.PIA.Player.Disable();
    }
    public void GetDialogue(Queue<Dialogue> dialogueSet)
    {
        Dialogue dialogue = dialogueSet.Dequeue();
        StartDialogue(dialogue);
    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;
        characterImage.sprite = dialogue.characterPortrait;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        count++;
        DisplayNextSentence();
        
    }

    public void DisplayNextSentence()
    {
        Debug.Log("Sentence Count: " + sentences.Count);
        Debug.Log("Dialogue Count: " + dialogueSet.Count);
        Debug.Log("Count: " + count);
        if (sentences.Count == 0 && dialogueSet.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    public void ClickBehavior() 
    {
        if (sentences.Count == 0 && dialogueSet.Count == 0)
        {
            EndDialogue();
            return;
        } else if (sentences.Count == 0) {
            GetDialogue(dialogueSet);
        } else {
            DisplayNextSentence();
        }
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        count = 0;
        PlayerController.Instance.PIA.Player.Enable();
<<<<<<< HEAD
        if (isEndScene) {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
=======
>>>>>>> 9bb65b7327e9ca7a5ed7c5598ff1bb2b86721c12
    }
}

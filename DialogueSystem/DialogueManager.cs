using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public List<string> DialogueSentence;
    public bool isDisplaying;
    public Dialogue DisplayingDialogue;
    public string Name;
    void Start()
    {
        
    }
    public void StartDialogue(Dialogue dialogue)
    {
        if (!isDisplaying)
        {
            DialogueSentence = new List<string>(dialogue.sentences);

        }
    }

}

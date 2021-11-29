using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public void TriggerDialogue()
    {
        DialogueUI Manager = GameObject.FindObjectOfType<DialogueUI>();
        Manager.StartDialogue(dialogue);
    }
}

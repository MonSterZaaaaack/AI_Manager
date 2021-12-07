using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool Enter = false;
    public void TriggerDialogue()
    {
        DialogueUI Manager = GameObject.FindObjectOfType<DialogueUI>();
        Manager.StartDialogue(dialogue);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Enter)
            {
                TriggerDialogue();
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Enter = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Enter = false;
        }
    }
}

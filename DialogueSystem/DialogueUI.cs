using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Gamekit3D;
using Gamekit3D.Message;


public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [Range(0,20)]
    [SerializeField] private float textSpeed;
    [SerializeField] private GameObject DialogueCanvas;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private GameObject Option1;
    [SerializeField] private GameObject Option2;
    private List<MessageType> Messages;
    private GameKitVersionCharacters Receiver;
    private bool OptionSelected;
    public void StartDialogue(Dialogue dialogue)
    {
        SelectedSet(false);
        textName.text = dialogue.Name;
        Messages = dialogue.Messages;
        Receiver = dialogue.Receiver;
        OpeningDialogueUI();
        StartCoroutine(ShowDialogue(dialogue.sentences));

    }
    public IEnumerator ShowDialogue(List<string> Text)
    {
        Debug.Log("Start Couroutine");
        PlayerInput.Instance.ReleaseControl();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        textLabel.text = string.Empty;
        float time = 0;
        int listindex = 0;
        int stringindex = 0;
        while (listindex < Text.Count)
        {
            string showText = Text[listindex];
            while (stringindex < showText.Length)
            {
                time += Time.unscaledDeltaTime * textSpeed;
                stringindex = Mathf.FloorToInt(time);
                stringindex = Mathf.Clamp(stringindex, 0, Text[listindex].Length);
                textLabel.text = showText.Substring(0, stringindex);
                yield return null;
            }
            Debug.Log(textLabel.text);
            Debug.Log("End Show text");
            OpenOptions(Text,listindex);
            Debug.Log("End Show Option");
            listindex += 3;
            stringindex = 0;
            time = 0;
            yield return new WaitUntil(() => SelectedGet());
        }
        PlayerInput.Instance.GainControl();
        ClosingDialogueUI();
        Debug.Log("End Courtine");
    }
    void OpenOptions(List<string> options,int index)
    {
        Option1.GetComponentInChildren <TMP_Text>().text = options[index + 1];
        Option2.GetComponentInChildren <TMP_Text>().text = options[index + 2];
        Option1.GetComponent<Button>().onClick.AddListener(() => TestMethod(0));
        Option2.GetComponent<Button>().onClick.AddListener(() => TestMethod(1));
    }
    void CloseOptions()
    {

    }
    bool SelectedGet()
    {
        return OptionSelected;
    }
    public void SelectedSet(bool result)
    {
        OptionSelected = result;
    }
    void OpeningDialogueUI()
    {
        DialogueCanvas.SetActive(true);
        Time.timeScale = 0;
        Option1.GetComponentInChildren<TMP_Text>().text = string.Empty;
        Option2.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }
    void ClosingDialogueUI()
    {
        DialogueCanvas.SetActive(false);
        textLabel.text = string.Empty;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

    }
    public void TestMethod(int n)
    {
        SelectedSet(true);
        Receiver.OnReceiveMessage(Messages[n], this, null);
    }
}

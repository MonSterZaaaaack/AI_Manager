using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D.Message;
[System.Serializable]
public class Dialogue
{
    [TextArea(3,10)]
    public string Name;
    public List<string> sentences;
    public List<MessageType> Messages;
    public GameKitVersionCharacters Receiver;

}

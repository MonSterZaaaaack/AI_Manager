using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Dialogue
{
    [TextArea(3,10)]
    public List<string> sentences;
    public string Name;

}
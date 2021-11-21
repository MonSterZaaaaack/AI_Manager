using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TraitsManager : EditorWindow
{
    // Start is called before the first frame update
    string[] Traits = new string[0];
    int traitnumber = -1;
    string traitname = "No Traits Selected";
    public static void ShowWindow()
    {
        var window = GetWindow<TraitsManager>();
        window.title = "Traits";
    }
    public void OnGUI()
    {
        GUILayout.BeginVertical();
        ShowTraits();
        if (Traits.Length > 0)
        {
            traitnumber = GUILayout.SelectionGrid(traitnumber, Traits, 1);
        }
        traitname = GUILayout.TextField(traitname);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Trait"))
        {
            AIManager.Instance.AddPersonalityTrait(traitname);
        }
        if (GUILayout.Button("Edit Trait"))
        {
            string oldname = Traits[traitnumber];
            AIManager.Instance.EditPersonalityTrait(oldname, traitname);
        }
        if (GUILayout.Button("Delete Trait"))
        {
            traitname = Traits[traitnumber];
            AIManager.Instance.DeletePersonalityTrait(traitname);
            traitname = "Trait Deleted";
        }
        GUILayout.EndHorizontal();
        if(GUILayout.Button("Save Chanegs"))
        {
            AIManager.Instance.Save();
        }
        if (GUILayout.Button("Back to Main Menu"))
        {
            AI_Manager.ShowWindow();
            Close();
        }
        GUILayout.EndVertical();
    }
    public void ShowTraits()
    {
        IList<string> CurrentTraits = AIManager.Instance.GetPersonalities();
        Traits = new string[CurrentTraits.Count];
        if (CurrentTraits.Count == 0)
        {
            GUILayout.Label("There's no Traits in the System, Please Add a new one");
        }
        else
        {
            for (int a = 0; a < CurrentTraits.Count; a++)
            {
                Traits[a] = CurrentTraits[a];
            }
        }
    }
}

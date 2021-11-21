using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CharacterManager : EditorWindow
{
    string[] Chars = new string[1] { "Click Get All Characters" };
    List<int> IDs;
    int charnumber = -1;
    int preselection = -1;
    string Charactername = "No Character Selected";
    Characters selected = null;
    int ID = 0;
    string classname = "No Character Selected";
    IList<int> Traits = new List<int>();
    GameObject obj = null;
    
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        var window = GetWindow<CharacterManager>();
        window.title = "Characters";
    }
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        // Show the list of characters
        GUILayout.BeginVertical();
        charnumber = GUILayout.SelectionGrid(charnumber, Chars, 1);
        // Select the Character need to be added
        ShowCharacter();
        if (charnumber != -1 && preselection != charnumber)
        {
            EditCharacter();
        }
        // Get all characters;
        if (GUILayout.Button("Back to Main Menu"))
        {
            AI_Manager.ShowWindow();
            Close();
        }
        GUILayout.EndVertical();
        if(selected != null)
        {
            // Show the data of the selected character
            GUILayout.BeginVertical();
            // Show the Character name
            GUILayout.BeginHorizontal();
            Charactername = GUILayout.TextField(Charactername);
            if (GUILayout.Button("Change Character Name"))
            {
                ChangeName();
                ShowCharacter();
            }
            GUILayout.EndHorizontal();
            //Show The rest data of the Character(ID,Class,etc.)
            GUILayout.BeginHorizontal();
            GUILayout.Label("Class Name: " + classname);
            GUILayout.Label("Character ID: " + ID);
            GUILayout.EndHorizontal();
            IList<string> TraitNames = new List<string>(AIManager.Instance.GetPersonalities());
            for(int i = 0; i < Traits.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(TraitNames[i] + " : ");
                Traits[i] = EditorGUILayout.IntField(Traits[i]);
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Edit Trait"))
            {
                selected.SetTraits(Traits);
                ShowCharacter();
            }
            if(GUILayout.Button("Save Changes"))
            {
                AIManager.Instance.Save();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    public void ShowCharacter()
    {
        HashSet<string> chars = new HashSet<string>(AIManager.Instance.GetAllCharacters());
        Chars = new string[chars.Count];
        if(Chars.Length == 0)
        {
            GUILayout.Label("There is no Characters Exists in the system");
        }
        else
        {
            int i = 0;
            foreach (string val in chars)
            {
                Chars[i] = val;
                i++;
            }
        }

    }
    public void EditCharacter()
    {
        obj = GameObject.Find(Chars[charnumber]);
        selected = obj.GetComponent<Characters>();
        Charactername = selected.GetName();
        classname = selected.GetClass().GetName();
        ID = selected.GetID();
        Traits = new List<int>(selected.GetTraits());
        if(preselection != charnumber)
        {
            preselection = charnumber;
            GUI.FocusControl(null);
        }


    }
    public void ChangeName()
    {
        HashSet<string> Managerlist = AIManager.Instance.GetAllCharacters();
        if (Managerlist.Contains(obj.name))
        {
            Managerlist.Remove(obj.name);
            Managerlist.Add(Charactername);
        }
        IList<string> Classlist = selected.GetClass().GetAllCharacters();
        if (Classlist.Contains(obj.name))
        {
            Classlist.Remove(obj.name);
            Classlist.Add(Charactername);
        }
        AIManager.Instance.SetCharList(Managerlist);
        selected.GetClass().SetCharacterlist(Classlist);
        obj.name = Charactername;
        selected.SetName(Charactername);
    }
}

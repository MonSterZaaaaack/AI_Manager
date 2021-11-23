using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System;

public class ClassManager : EditorWindow
{
    string[] classnames = new string[0];
    string[] charnames = new string[1] { "testCharacter" };
    string[] eventnames = new string[1] { "testEvent" };
    string[] traitsnames = new string[1] { "testTrait" };
    string[] availableEventsNames = new string[1] { "Available events" };
    IList<AIEvents> availableEvents = new List<AIEvents>();
    IList<AIEvents> classEvents = new List<AIEvents>();
    string newCharName = "Please Enter the Name of The New Character";
    CharacterClass selected;
    int[,] ranges = new int[1, 2] { { 0, 0 } };
    int addevent = 0;
    int classes = -1;
    int preselection = -1;
    int characters = 0;
    int events = 0;
    int traits = 0;
    string selectedname = "";
    public Vector2 Scrollpos;
    public ListView view;
    string classname = "Class Name";
    // Start is called before the first frame update
    [MenuItem("Asset/ClassTest")]
    public static void ShowWindow()
    {
        var window = GetWindow<ClassManager>();
        window.title = "Classes";
    }
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        // Start print all available classes;
        // Start print the data of the selected class;
        RefreshClasses();
        classes = GUILayout.SelectionGrid(classes, classnames, 1);
        if(classes != -1 && classes != preselection)
        {
            EditClass();
        }
        else if(classes == -1)
        {
            ShowSampleClass();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Class Name:");
        ShowClassName();
        GUILayout.Label("Characters in this class:");
        ShowCharacters();
        GUILayout.Label("AIEvents in this class:");
        ShowEvents();
        GUILayout.Label("Personality Traits Settings:");
        ShowTraits();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Class"))
        {
            AddClass.ShowWindow();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Back to Main Menu"))
        {
            AI_Manager.ShowWindow();
            Close();
        }
        if(GUILayout.Button("Delete Class"))
        {
            AIManager.Instance.DeleteClass(selectedname);
            RefreshClasses();
            classes = -1;
            preselection = -1;
            AIManager.Instance.Save();

        }
        if (GUILayout.Button("Save changes"))
        {
            AIManager.Instance.Save();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        if (AIManager.Instance.getCreated())
        {
            EditClass();
            AIManager.Instance.setnewChar(false);
        }
    }
    void ShowSampleClass()
    {
        classname = "SampleClass";
        selectedname = classname;
        selected = null;
        classEvents = new List<AIEvents>(); ;
        availableEvents = new List<AIEvents>();
        availableEventsNames = new string[availableEvents.Count];
        charnames = new string[0];
        eventnames = new string[0];
        IList<string> PTs = new List<string>();
        ranges = new int[0,0];
        traitsnames = new string[0];

    }
    public void CreateClasses()
    {
        if(AIManager.Instance != null)
        {
            for (int i = 0; i < 10; i++)
            {
                string name = i.ToString();
                CharacterClass temp = new CharacterClass(name, 5);
                AIManager.Instance.AddClass(temp);
            }
        }
    }
    // Used to Show All classes available in the system.
    public void RefreshClasses()
    {
        IList<CharacterClass> classes = AIManager.Instance.GetAllClass();
        classnames = new string[classes.Count];
        if(classes.Count == 0)
        {
            GUILayout.Label("No Classes in the system Right now");
        }
        else
        {
            for (int a = 0; a < classes.Count; a++)
            {
                classnames[a] = classes[a].GetName();
            }
        }
    }
    // Use to show All information of the selected Class.
    public void EditClass()
    {
        classname = classnames[classes];
        selectedname = classname;
        selected = AIManager.Instance.GetClass(classname);
        classEvents = selected.GetEvents();
        IList<AIEvents> temp = AIManager.Instance.GetEvents();
        availableEvents = new List<AIEvents>();
        for(int i = 0; i < temp.Count; i++)
        {
            if (classEvents.Contains(temp[i]))
            {
                continue;
            }
            else
            {
                availableEvents.Add(temp[i]);
            }
        }
        availableEventsNames = new string[availableEvents.Count];
        for(int i = 0; i < availableEvents.Count; i++)
        {
            availableEventsNames[i] = availableEvents[i].GetName();
        }

        charnames = new string[selected.GetAllCharacters().Count];
        eventnames = new string[classEvents.Count];
        for (int i = 0; i < selected.GetAllCharacters().Count; i++)
        {
            charnames[i] = selected.GetAllCharacters()[i];

        }
        for (int i = 0; i < classEvents.Count; i++)
        {
            eventnames[i] = classEvents[i].GetName();

        }
        IList<string> PTs = AIManager.Instance.GetPersonalities();
        ranges = new int[PTs.Count,2];
        traitsnames = new string[PTs.Count];
        for (int i = 0; i < PTs.Count; i++)
        {
            traitsnames[i] = PTs[i];
            ranges[i,0] = selected.GetPersonalityTraits()[i][0];
            ranges[i, 1] = selected.GetPersonalityTraits()[i][1];
        }
        if(classes != preselection)
        {
            preselection = classes;
            GUI.FocusControl(null);
        }
    }
    public void ShowClassName()
    {
        GUILayout.BeginHorizontal();
        classname = GUILayout.TextField(classname);
        if (GUILayout.Button("Save Class Name"))
        {
            AIManager.Instance.SetClassName(classname, selectedname);
            classnames[classes] = classname;
        }
        GUILayout.EndHorizontal();
    }
    public void ShowCharacters()
    {
        
        GUILayout.BeginVertical();
        characters = GUILayout.SelectionGrid(characters, charnames, 3);
        GUILayout.BeginHorizontal();
        newCharName = GUILayout.TextField(newCharName);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add Character"))
        {
            AIManager.Instance.SetNewCharInfo(selected);
            AddCharacter.ShowWindow();
        }
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Edit Character"))
        {
            Characters temp = GameObject.Find(selected.GetAllCharacters()[characters]).GetComponent<Characters>();
            temp.SetName(newCharName);
            EditClass();
        }
        if (GUILayout.Button("Delete Character"))
        {
            string temp = selected.GetAllCharacters()[characters];
            selected.DeleteCharacters(temp);
            SaveChanges();
            AIManager.Instance.Save();
            EditClass();

        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    public void ShowEvents()
    {
        events = GUILayout.SelectionGrid(events, eventnames, 3);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        addevent = EditorGUILayout.Popup(addevent, availableEventsNames);
        if (GUILayout.Button("Add Event"))
        {
            selected.AddEvents(availableEvents[addevent]);
            EditClass();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Delete Event"))
        {
            AIEvents temp = selected.GetEvents()[events];
            selected.DeleteEvents(temp);
            EditClass();

        }
        GUILayout.EndVertical();
    }
    public void ShowTraits()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        for (int i = 0; i < traitsnames.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(traitsnames[i]);
            ranges[i,0] = EditorGUILayout.IntField(ranges[i,0]);
            ranges[i,1] = EditorGUILayout.IntField(ranges[i,1]);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        if (GUILayout.Button("Edit Traits"))
        {
            IList<IList<int>> temp = new List<IList<int>>();
            for (int i = 0; i < ranges.GetLength(0); i++)
            {
                IList<int> val = new List<int>();
                if (ranges[i, 0] > ranges[i, 1])
                {
                    ranges[i, 0] = ranges[i, 1];
                }
                val.Add(ranges[i,0]);
                val.Add(ranges[i,1]);
                temp.Add(val);
            }
            selected.setTraitRange(temp);
            EditClass();

        }
        GUILayout.EndHorizontal();

    }
    public void SaveChanges()
    {
        string[] path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
        bool saveOK = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), string.Join("/", path));
        Debug.Log("Saved Scene " + (saveOK ? "OK" : "Error!"));
    }
}

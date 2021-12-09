using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class EventManager : EditorWindow
{
    string[] Events = new string[0];
    string[] Actions = new string[1] { "Update Event Actions" };
    int Actionnumber = 0;
    int Eventnumber = -1;
    int preselection = -1;
    string Eventname = "No Event Selected";
    string EventCondition = "No Event Selected";
    DemoAction demo;
    AIEvents selected = null;
    IList<int> Factors = null;
    IList<AIEvents> CurrentEvents;
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        var window = GetWindow<EventManager>();
        window.title = "Events";
    }
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ShowEvents();
        Eventnumber = GUILayout.SelectionGrid(Eventnumber, Events, 1);
        if(GUILayout.Button("Add New Event"))
        {
            AddEvent();
        }
        if(Eventnumber != -1 && Eventnumber != preselection)
        {
            SelectEvent();
        }
        /*
         * Replace the Event names saved in the system with the new list of event names currently available.
         */
        if (GUILayout.Button("Reset"))
        {
            HashSet<string> names = new HashSet<string>();
            IList<AIEvents> eventlist = AIManager.Instance.GetEvents();
            for(int i = 0; i < eventlist.Count; i++)
            {
                names.Add(eventlist[i].GetName());
            }
            AIManager.Instance.SetEventNames(names);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        ShowDetail();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Back To Main and Save"))
        {
            AIManager.Instance.Save();
            AI_Manager.ShowWindow();
            Close();
        }
        
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    public void ShowDetail()
    {
        if(selected != null)
        {
            // Show the data stored in the selected events;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Event Name : ");
            Eventname = GUILayout.TextField(Eventname);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Condition: " + selected.getCondition().getName());
            EventCondition = GUILayout.TextField(EventCondition);
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Set EventCondition"))
            {
                TestCondition newcondition = new TestCondition(EventCondition);
                selected.SetCondition(newcondition);
            }
            // Show Actions;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Event Action");
            Actionnumber = EditorGUILayout.Popup(Actionnumber, Actions);
            GUILayout.EndHorizontal();
            GUILayout.Label("Action nummber : "+ Actionnumber.ToString());
            GUILayout.Label("Action name : " + selected.getActionName());
            // Show Traits Factors;
            IList<string> TraitNames = new List<string>(AIManager.Instance.GetPersonalities());
            for (int i = 0; i < Factors.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(TraitNames[i]);
                Factors[i] = EditorGUILayout.IntField(Factors[i]);
                GUILayout.EndHorizontal();
            }
            HashSet<string> eventNames = AIManager.Instance.GetEventNames();
            if (eventNames.Contains(selected.GetName()))
            {
                GUILayout.Label("Event already in System");
            }
            else
            {
                if (GUILayout.Button("Add Event"))
                {
                    AIManager.Instance.CreateEvent(selected);
                    ShowEvents();
                }
            }
            if (GUILayout.Button("Save Changes"))
            {
                string oldname = selected.GetName();
                selected.SetActionNumber(Actionnumber);
                selected.SetActionName(Actions[Actionnumber]);
                selected.SetFactor(Factors);
                if(oldname != Eventname)
                {
                    if(AIManager.Instance.ChangeEventName(oldname, Eventname))
                    {
                        selected.SetName(Eventname);
                    }
                }
                ShowEvents();
            }
            if(GUILayout.Button("Delete Event"))
            {
                AIManager.Instance.DeleteEvent(selected.GetName());
                selected = null;
                ShowEvents();
            }
            GUILayout.EndVertical();
        }
    }
    public void SelectEvent()
    {
        selected = CurrentEvents[Eventnumber];
        Eventname = selected.GetName();
        EventCondition = selected.getCondition().getName();
        demo = AIManager.Instance.GetActions();
        Dictionary<string, MethodInfo> methods = demo.AvailableMethods;
        Actions = new string[methods.Count];
        int i = 0;
        foreach(MethodInfo val in methods.Values)
        {
            Actions[i] = val.Name;
            i++;
        }
        Factors = selected.GetFactor();
        string ActionName = selected.getActionName();
        Actionnumber = selected.GetActionNumber();
        if(Actionnumber != Array.IndexOf(Actions, ActionName))
        {
            Actionnumber = Array.IndexOf(Actions, ActionName);
            selected.SetActionNumber(Actionnumber);
        }
        if(Eventnumber != preselection)
        {
            GUI.FocusControl(null);
            preselection = Eventnumber;
        }
        
    }
    public void AddEvent()
    {
        int size = AIManager.Instance.GetPersonalities().Count;
        selected = new AIEvents("NewEvent", size);
        TestCondition newcon = new TestCondition("NoCondition");
        selected.SetCondition(newcon);
        Eventname = selected.GetName();
        DemoAction demo = selected.GetAction();
        IList<MethodInfo> methods = new List<MethodInfo>(demo.methodslist);
        Actions = new string[methods.Count];
        for (int i = 0; i < methods.Count; i++)
        {
            Actions[i] = methods[i].Name;
        }
        Factors = selected.GetFactor();
        Actionnumber = selected.GetActionNumber();
    }
    public void ShowEvents()
    {
        CurrentEvents = AIManager.Instance.GetEvents();
        Events = new string[CurrentEvents.Count];
        if (Events.Length == 0)
        {
            GUILayout.Label("There is no Event exist in the system");
        }
        else
        {
            for (int a = 0; a < CurrentEvents.Count; a++)
            {
                Events[a] = CurrentEvents[a].GetName();
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
[Serializable]
public class AIEvents
{
    private string Name;
    private IList<int> TraitsFactor;
    private int Weight;
    private int ActionNumber;
    private string ActionName;
    private DemoAction demo;
    private EventCondition condition;
    /*
     * AIEvents are defines the response of character to certain stimulus in game.
     * Each event would have its own Condition, once the condition is meet, the event would be selected and return to the character by the Character Class.
     */
    public AIEvents(string name,int size)
    {
        Name = name;
        demo = AIManager.Instance.GetActions();
        TraitsFactor = new List<int>();
        for(int i = 0; i < size; i++)
        {
            TraitsFactor.Add(0);
        }
        Weight = 0;
        ActionNumber = 0;
    }
    public void SetCondition(EventCondition newcondition)
    {
        condition = newcondition;
    }
    public void SetName(string name)
    {
        Name = name;
    }
    public string GetName()
    {
        return Name;
    }
    public void SetFactor(IList<int> newFactor)
    {
        TraitsFactor = new List<int>(newFactor);
    }
    public IList<int> GetFactor()
    {
        return TraitsFactor;
    }
    public void SetActionNumber(int num)
    {
        ActionNumber = num;
    }
    public int GetActionNumber()
    {
        return ActionNumber;
    }
    public void SetActionName(string name)
    {
        ActionName = name;
    }
    public string getActionName()
    {
        return ActionName;
    }
    public void SetWeight(int wei)
    {
        Weight = wei;
    }
    public DemoAction GetAction()
    {
        return demo;
    }
    public void SetAction(DemoAction newDemo)
    {
        demo = newDemo;
    }
    public void Do(Characters[] Targets)
    {
        demo = AIManager.Instance.GetActions();
        MethodInfo val = demo.AvailableMethods[ActionName];
        object[] parameters = new object[] { Targets[0] };
        val.Invoke(demo,parameters);
    }
    public bool ExamCondition(EventCondition con)
    {
        return condition.Examcondition(con);
    }
    public EventCondition getCondition()
    {
        return condition;
    }

}

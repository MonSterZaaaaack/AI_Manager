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
    private DemoAction demo;
    private EventCondition condition;
    public AIEvents(string name,int size)
    {
        Name = name;
        demo = new DemoAction();
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
    public void test()
    {
        Debug.Log("TestStart");
        MethodInfo val = null;
        foreach(MethodInfo temp in demo.methodslist)
        {
            if (temp.Name.Equals("TestAction1"))
            {
                val = temp;
            }
        }
        Debug.Log(demo.methodslist.Count);
        val.Invoke(demo, null);
        Debug.Log("TestEnds");

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
    public void SetWeight(int wei)
    {
        Weight = wei;
    }
    public DemoAction GetAction()
    {
        return demo;
    }
    public void Do(Characters[] Targets)
    {
        MethodInfo val = demo.methodslist[ActionNumber];
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

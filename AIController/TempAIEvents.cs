using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
abstract public class TempAIEvents 
{
    private string Name;
    private EventCondition Condition;
    private Action Action;
    private IList<Characters> Targets;
    private IList<int> TraitsFactor;
    private int Weight;
    public TempAIEvents()
    {
        Name = "NoName";
        Condition = null;
        Action = null;
        Targets = new List<Characters>();
        TraitsFactor = new List<int>();
        Weight = -1;
    }
    public TempAIEvents(string name, EventCondition condition, Action action,IList<int> Factors)
    {
        Name = name;
        Condition = condition;
        Action = action;
        Targets = new List<Characters>();
        TraitsFactor = Factors;
        Weight = -1;
    }
    public bool ExamCondition(EventCondition cons)
    {
        return true;
    }
    public string GetName()
    {
        return Name;
    }
    public void SetName(string name)
    {
        Name = name;
    }
    public void SetAction(Action action)
    {
        Action = action;
    }
    public Action GetAction()
    {
        return Action;
    }
    public void SetCondition(EventCondition condition)
    {
        Condition = condition;
    }
    public EventCondition GetCondition()
    {
        return Condition;
    }
    public void SetFactor(IList<int> factors)
    {
        TraitsFactor.Clear();
        for(int i = 0; i < factors.Count; i++)
        {
            TraitsFactor.Add(factors[i]);
        }
    }
    public IList<int> GetFactor()
    {
        return TraitsFactor;
    }
    public void SetTarget(IList<Characters> charlist)
    {
        Targets = new List<Characters>(charlist);
    }
    public int GetWeight()
    {
        return Weight;
    }
    public void SetWeight(int w)
    {
        Weight = w;
    }
    public void Do()
    {
        
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEvent1
{
    private string Name;
    private EventCondition Condition;
    private Action_1 Action;
    private IList<Characters> Targets;
    private IList<int> TraitsFactor;
    private int Weight;
    private Characters target;
    private int ActionNumber;
    public SampleEvent1(string name, IList<int> Factors)
    {
        Name = name;
        target = null;
    }
    new public void SetTarget(IList<Characters> temp)
    {
        target = temp[0];
    }

    new public void Do()
    {
        Debug.Log(target.GetName()+" : SampleEvent1");
    }

}

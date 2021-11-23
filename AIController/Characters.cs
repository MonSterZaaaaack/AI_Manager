using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gamekit3D;
using Gamekit3D.Message;
[Serializable]
public abstract class Characters : MonoBehaviour
{
    public string Name;
    public int ID;
    public CharacterClass Class;
    public string ClassName;
    public List<int> PersonalityTraits;
    public Damageable.DamageMessage lastestMessage;
    public Characters(string Name,int ID,CharacterClass Class)
    {
        this.Name = Name;
        this.ID = ID;
        this.Class = Class;
        PersonalityTraits = new List<int>();
        for(int i = 0; i < Class.GetPersonalityTraits().Count; i++)
        {
            PersonalityTraits.Add(UnityEngine.Random.Range(Class.GetPersonalityTraits()[i][0], Class.GetPersonalityTraits()[i][1]));
        }
    }
    public void UpdateCharacter()
    {
        PersonalityTraits = new List<int>();
        CharacterClass thisclass = AIManager.Instance.GetClass(ClassName);
        Debug.Log(thisclass.GetName());
        for (int i = 0; i < thisclass.GetPersonalityTraits().Count; i++)
        {
            PersonalityTraits.Add(UnityEngine.Random.Range(Class.GetPersonalityTraits()[i][0], Class.GetPersonalityTraits()[i][1]));
        }
    }
    public void SetClassName(string Classname)
    {
        ClassName = Classname;
    }
    public string GetClassName()
    {
        return ClassName;

    }
    public string GetName()
    {
        return Name;
    }
    public void SetName(string Name)
    {
        this.Name = Name;
    }
    public CharacterClass GetClass()
    {
        return Class;
    }
    public void SetClass(CharacterClass newclass)
    {
        Class = newclass;
    }
    public int GetID()
    {
        return ID;
    }
    public IList<int> GetTraits()
    {
        return PersonalityTraits;
    }
    public void SetTraits(IList<int> newTraits)
    {
        PersonalityTraits = new List<int>(newTraits);
    }
    public void GetEvent(EventCondition con)
    {
        CharacterClass thisclass = AIManager.Instance.GetClass(ClassName);
        IList<AIEvents> temp = new List<AIEvents>(thisclass.GetEvents());
        IList<AIEvents> val = new List<AIEvents>();
        EnemyBehavior test = gameObject.GetComponent<ChomperBehavior>();
        foreach(AIEvents events in temp)
        {
            if (events.ExamCondition(con))
            {
                val.Add(events);
            }
        }
        AIEvents selected = Randomize(val);
        Characters[] targets = new Characters[] { this };
        if(selected == null)
        {
            return;
        }
        else
        {
            selected.Do(targets);
        }

    }
    public int TestGetEvent(EventCondition con)
    {
        IList<AIEvents> temp = new List<AIEvents>(Class.GetEvents());
        IList<AIEvents> val = new List<AIEvents>();
        foreach (AIEvents events in temp)
        {
            if (events.ExamCondition(con) && events.GetFactor().Count == PersonalityTraits.Count)
            {
                val.Add(events);
            }
        }
        AIEvents selected = Randomize(val);
        return selected.GetActionNumber();

    }
    public AIEvents Randomize(IList<AIEvents> events)
    {
        IList<int> Weights = new List<int>();
        for(int i = 0; i < events.Count; i++)
        {
            int Weight = 0;
            for(int j = 0; j < PersonalityTraits.Count; j++)
            {
                Weight = (int)Math.Round((double)PersonalityTraits[j]*(double)events[i].GetFactor()[j])+Weight;
            }
            Weights.Add(Weight);
        }
        IList<AIEvents> randomList = new List<AIEvents>();
        for(int i = 0; i < Weights.Count; i++)
        {
            for (int j = 0; j < Weights[i]; j++)
            {
                randomList.Add(events[i]);
            }
        }
        int index = UnityEngine.Random.Range(0,randomList.Count);
        if(randomList.Count == 0)
        {
            return null;
        }
        else
        {
            return randomList[index];
        }
    }    
    public void Updatecharacterinfo(IList<IList<int>> newTraits)
    {
        CharacterClass Class = AIManager.Instance.GetClass(ClassName);
        PersonalityTraits = new List<int>();
        for (int i = 0; i < Class.GetPersonalityTraits().Count; i++)
        {
            PersonalityTraits.Add(UnityEngine.Random.Range(newTraits[i][0], newTraits[i][1]));
        }
    }

}

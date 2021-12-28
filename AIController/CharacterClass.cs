using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class CharacterClass
{
    private string Name;
    private IList<AIEvents> Events;
    private IList<IList<int>> Traits;
    private IList<string> CharacterList;

    
    /*
     * Character Class is a container for groups of characters created by AI_Behaviour controller
     * Each Character in the should belongs to at least one(and only one for this version) of character classes
     * The Class will define the sets of Available events and the personality trait value ranges for the Character.
     */
    public CharacterClass(string Name, int size)
    {
        this.Name = Name;
        Traits = new List<IList<int>>();
        Events = new List<AIEvents>();
        CharacterList = new List<string>();
    }
    public void AddEvents(AIEvents events)
    {
        Events.Add(events);
    }
    public void DeleteEvents(AIEvents events)
    {
        Events.Remove(events);
    }
    public void AddCharacters(string val)
    {
        CharacterList.Add(val);
    }
    public void DeleteCharacters(string val)
    {
        CharacterList.Remove(val);
        AIManager.Instance.DeleteCharacter(val);
    }
    public IList<IList<int>> GetPersonalityTraits()
    {
        return Traits;
    }
    public void setTraitRange(IList<IList<int>> range)
    {
        Traits = new List<IList<int>>(range);
        Notify();
    }
    public IList<AIEvents> GetEvents()
    {
        return Events;
    }
    public void SetName(string name)
    {
        Name = name;
    }
    public string GetName()
    {
        return Name;
    }
    public Characters GetCharacters(string name)
    {
        GameObject obj = GameObject.Find(name);
        if(obj == null)
        {
            return null;
        }
        else
        {
            Characters val = obj.GetComponent<Characters>();
            return val;
        }
    }
    public IList<string> GetAllCharacters()
    {
        return CharacterList;
    }
    public void SetCharacterlist(IList<string> newList){
        CharacterList = new List<string>(newList);
    }
    public void Notify()
    {
        GameObject obj = null;
        for(int i = 0; i < CharacterList.Count; i++)
        {
            obj = GameObject.Find(CharacterList[i]);
            if (obj == null)
            {
                continue;
            }
            else
            {
                Characters val = obj.GetComponent<Characters>();
                val.Updatecharacterinfo(GetPersonalityTraits());
            }
        }
    }
}

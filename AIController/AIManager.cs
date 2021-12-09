using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
[ExecuteAlways]
public class AIManager : MonoBehaviour
{
    private Dictionary<string, CharacterClass> Classlist;
    private HashSet<string> Characters;
    private HashSet<string> Eventnames;
    private List<AIEvents> Eventlist;
    private IList<string> PersonalityTraits;
    private HashSet<int> AvailableIDs;
    private int CharacterSize;
    private string OnCreateCharName;
    private CharacterClass OnCreateCharClass;
    private bool newCharcreated;
    private Scene newScene;
    private static AIManager I_instance;
    private DemoAction actions;
    public static AIManager Instance { get { return I_instance; } }
    public int gettest()
    {
        return CharacterSize;
    }
    public int getCharSize()
    {
        return CharacterSize;
    }
    /* 
     * The AI Manager uses Singleton Design pattern to make sure there would be only one instance of AI Manager exists at the same time.
     * So when it enters the Playing mode or Editing mode, it would first check whether a instance of AI Manager already exists, if so,
     * Destory this object,create a new instance of AI Manager by loading all of the data in the saving file
     * If there is no saving file exists, create a new one.
     */
    private void Awake()
    {
        if (I_instance == null)
        {
            newScene = SceneManager.GetActiveScene();

            if (File.Exists(Application.persistentDataPath + "/AIManager" + newScene.name + ".dat"))
            {
                Debug.Log(newScene.name);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/AIManager" + newScene.name + ".dat", FileMode.Open);
                ManagerData tempInstance = bf.Deserialize(fs) as ManagerData;
                I_instance = this;
                Classlist = new Dictionary<string, CharacterClass>(tempInstance.Classlist);
                Characters = new HashSet<string>(tempInstance.Characters);
                Eventnames = new HashSet<string>(tempInstance.Eventnames);
                Eventlist = new List<AIEvents>(tempInstance.Eventlist);
                PersonalityTraits = new List<string>(tempInstance.PersonalityTraits);
                AvailableIDs = new HashSet<int>(tempInstance.AvailableIDs);
                CharacterSize = tempInstance.CharacterSize;
                newCharcreated = false;
                actions = new DemoAction();
                fs.Close();

            }
            else
            {
                Debug.Log("New Controller");
                I_instance = this;
                Classlist = new Dictionary<string, CharacterClass>();
                Characters = new HashSet<string>();
                Eventnames = new HashSet<string>();
                Eventlist = new List<AIEvents>();
                PersonalityTraits = new List<string>();
                AvailableIDs = new HashSet<int>();
                CharacterSize = 2;
                newCharcreated = false;
                for (int i = 0; i < CharacterSize; i++)
                {
                    AvailableIDs.Add(i);
                }
                actions = new DemoAction();
            }
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("AI_Manager Initialized");
    }
    /* 
     * Check whether the AIManager instance still exists, if not, create a new one.
     */ 
    void Update()
    {
        if (AIManager.Instance == null)
        {
            newScene = SceneManager.GetActiveScene();

            if (File.Exists(Application.persistentDataPath + "/AIManager" + newScene.name + ".dat"))
            {
                Debug.Log(newScene.name);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/AIManager" + newScene.name + ".dat", FileMode.Open);
                ManagerData tempInstance = bf.Deserialize(fs) as ManagerData;
                I_instance = this;
                Classlist = new Dictionary<string, CharacterClass>(tempInstance.Classlist);
                Characters = new HashSet<string>(tempInstance.Characters);
                Eventnames = new HashSet<string>(tempInstance.Eventnames);
                Eventlist = new List<AIEvents>(tempInstance.Eventlist);
                PersonalityTraits = new List<string>(tempInstance.PersonalityTraits);
                AvailableIDs = new HashSet<int>(tempInstance.AvailableIDs);
                CharacterSize = tempInstance.CharacterSize;
                newCharcreated = false;
                actions = new DemoAction();
                fs.Close();

            }
            else
            {
                Debug.Log("New Controller");
                I_instance = this;
                Classlist = new Dictionary<string, CharacterClass>();
                Characters = new HashSet<string>();
                Eventnames = new HashSet<string>();
                Eventlist = new List<AIEvents>();
                PersonalityTraits = new List<string>();
                AvailableIDs = new HashSet<int>();
                CharacterSize = 2;
                newCharcreated = false;
                for (int i = 0; i < CharacterSize; i++)
                {
                    AvailableIDs.Add(i);
                }
                actions = new DemoAction();
            }
            Debug.Log("AI_Manager Re_Initialized");
        }
    }
    public DemoAction GetActions()
    {
        return actions;
    }
    /*
     * Add a new class to the System.
     * If the new class's name already exist in the system, igonore the new class
     * Otherwise add the new created class to the system.
     */
    public void AddClass(CharacterClass newclass)
    {
        if (!Classlist.ContainsKey(newclass.GetName()))
        {
            IList<IList<int>> temp = new List<IList<int>>();
            for (int i = 0; i < PersonalityTraits.Count; i++)
            {
                IList<int> val = new List<int>();
                val.Add(0);
                val.Add(0);
                temp.Add(val);
            }
            newclass.setTraitRange(temp);
            Classlist.Add(newclass.GetName(), newclass);
        }
        else
        {
            Debug.LogError("The name of the new Class already exist in the system.\nAdd new Class Failed");
        }
    }
    /*
     * Return the CharacterClass by Name
     */
    public CharacterClass GetClass(string Name)
    {
        if (Classlist.ContainsKey(Name))
        {
            return Classlist[Name];
        }
        else
        {
            Debug.LogError("Can not find Class: " + Name + " in the System");
            return null;
        }
    }
    /*
     * Return the list with all character classes in the system
     */
    public IList<CharacterClass> GetAllClass()
    {
        IList<CharacterClass> temp = new List<CharacterClass>();
        foreach (CharacterClass val in Classlist.Values)
        {
            temp.Add(val);
        }
        return temp;
    }
    /*
     * Change the name of a class.
     * If the new name already exists in the system
     * Reject the change name request
     */
    public void SetClassName(string Name, string oldName)
    {
        if (Classlist.ContainsKey(oldName))
        {
            if (!Classlist.ContainsKey(Name))
            {
                CharacterClass temp = Classlist[oldName];
                temp.SetName(Name);
                Classlist.Remove(oldName);
                Classlist.Add(Name, temp);
            }
            else
            {
                Debug.LogError("The new Name already exist in the system");
            }
        }
        else
        {
            Debug.LogError("Can not find Class: " + oldName + " In the system");
        }
    }
    /*
     * Delete the Character class by name.
     * If the class exist in the system, then delete all characters in this class first.
     * And then remove this class from the system.
     */
    public void DeleteClass(string Name)
    {
        if (Classlist.ContainsKey(Name))
        {
            CharacterClass OnDeleteClass = Classlist[Name];
            IList<string> CharList = new List<string>(OnDeleteClass.GetAllCharacters());
            foreach (string CharName in CharList)
            {
                OnDeleteClass.DeleteCharacters(CharName);
            }
            Classlist.Remove(Name);
        }
        else
        {
            Debug.LogError("The Class does not exist in the system");
        }
    }
    /*
     * Add a new Personality Trait to the system
     * If the name of the New Personality Trait already exist in the system,
     * reject the request.
     * Else, notify all the classes about the change in Personality Traits list.
     */
    public void AddPersonalityTrait(string Name)
    {
        if (PersonalityTraits.Contains(Name))
        {
            Debug.LogError("The name of the new Personality Traits already exist in the System\nAdd new Personality trait failed");
            return;
        }
        PersonalityTraits.Add(Name);
        notify(PersonalityTraits.Count - 1, true);
        foreach (AIEvents tempevent in Eventlist)
        {
            IList<int> templist = tempevent.GetFactor();
            templist.Add(0);
        }
    }
    /*
     * Delete the Personality trait from the system
     */
    public void DeletePersonalityTrait(string Name)
    {
        if (PersonalityTraits.Contains(Name))
        {
            int index = PersonalityTraits.IndexOf(Name);
            PersonalityTraits.Remove(Name);
            notify(index, false);
            foreach (AIEvents tempevent in Eventlist)
            {
                IList<int> templist = tempevent.GetFactor();
                templist.RemoveAt(index);
            }
        }
        else
        {
            Debug.LogError("The name of the Personality Traits does not exist in the System\n Delete Personality trait failed");
        }
    }
    /*
     * Edit the Name of the personality trait from the system.
     * If the new Name already exist in the system,
     * Reject the request
     */
    public void EditPersonalityTrait(string Name, string newName)
    {
        if (PersonalityTraits.Contains(Name))
        {
            if (PersonalityTraits.Contains(newName))
            {
                Debug.LogError("The new name of the Personality Traits already exist in the System\n Edit Personality trait failed");
                return;
            }
            int index = PersonalityTraits.IndexOf(Name);
            PersonalityTraits[index] = newName;
        }
        else
        {
            Debug.LogError("The name of the Personality Traits does not exist in the System\n Edit Personality trait failed");
        }
    }
    /*
     * Notify all classes in the system about the change of the personality trait.
     * If a new personality trait is added to the system, set the ranges of the new Trait values to (0,0)
     * And then the class will update the new ranges of personality traits to all characters in this class.
     * If a personality trait has been deleted from the system.
     * Notify all the classes first and then notify all characters in the system,
     */
    public void notify(int index, bool add)
    {
        if (add)
        {
            foreach (CharacterClass classes in Classlist.Values)
            {
                IList<IList<int>> temp = new List<IList<int>>(classes.GetPersonalityTraits());
                IList<IList<int>> curr = new List<IList<int>>(temp);
                IList<int> newtrait = new List<int>();
                newtrait.Add(0);
                newtrait.Add(0);
                curr.Add(newtrait);
                classes.setTraitRange(curr);
                classes.Notify();

            }
        }
        else
        {
            foreach (CharacterClass classes in Classlist.Values)
            {
                IList<IList<int>> temp = new List<IList<int>>(classes.GetPersonalityTraits());
                temp.RemoveAt(index);
                classes.setTraitRange(temp);
                classes.Notify();
            }
        }
    }
    /*
     * Return the list of all personality traits
     */
    public IList<string> GetPersonalities()
    {
        return PersonalityTraits;
    }
    /*
     * Delete the Event From the system by name.
     * And then delete this event from all classes.
     */
    public void DeleteEvent(string Name)
    {
        if (Eventnames.Contains(Name))
        {
            Eventnames.Remove(Name);
            AIEvents tempevent = null;
            for (int i = 0; i < Eventlist.Count; i++)
            {
                if (Eventlist[i].GetName().Equals(Name))
                {
                    tempevent = Eventlist[i];
                    Eventlist.Remove(tempevent);
                    break;
                }
            }
            foreach (CharacterClass val in Classlist.Values)
            {
                val.DeleteEvents(tempevent);
            }
        }
        else
        {
            Debug.LogError("Delete Event Failed. The Given Event name does not exist in the System");
        }
    }
    /*
     * Create a new Event
     * Reject the request if the name of the Event already exist in the system.
     */
    public void CreateEvent(AIEvents events)
    {
        if (!Eventnames.Contains(events.GetName()))
        {
            Eventnames.Add(events.GetName());
            Eventlist.Add(events);
        }
        else
        {
            Debug.LogError("Create Event Failed. The Name of the Event Already Exist in the System");
        }
    }
    /*
     * Get the list of all events in the system
     */
    public IList<AIEvents> GetEvents()
    {
        return Eventlist;
    }
    /*
     * Get the list of all Event Names in the system.
     */
    public HashSet<string> GetEventNames()
    {
        return Eventnames;
    }
    /*
     * Set the name of all events in the system
     */
    public void SetEventNames(HashSet<string> names)
    {
        Eventnames = new HashSet<string>(names);
    }
    /*
     * Change the Name of the given event.
     * If the Name of event does not exist in the system or the new name already exist in the system, log error
     * Else set the new name to the event.
     */
    public bool ChangeEventName(string oldname, string newname)
    {
        if (Eventnames.Contains(oldname))
        {
            if (!Eventnames.Contains(newname))
            {
                Eventnames.Remove(oldname);
                Eventnames.Add(newname);
                foreach (AIEvents val in Eventlist)
                {
                    if (val.GetName().Equals(oldname))
                    {
                        val.SetName(newname);
                    }
                }
            }
            else
            {
                Debug.LogError("Change Event Name Failed.\n The new Event Name Already Exist in the system");
                return false;
            }
        }
        else
        {
            Debug.LogError("Change Event Name Failed.\n The Name of the Event does not exist in the system");
            return false;
        }
        return true;
    }
    /*
     * Create a new Character by name
     */
    public void CreateCharacters(string Name)
    {
        Characters.Add(Name);
    }
    /*
     * Set the class of the new created character.
     * used to create new Characters
     */
    public void SetNewCharInfo(CharacterClass newclass)
    {
        OnCreateCharClass = newclass;
    }
    /*
     * Used to create new Characters
     */
    public CharacterClass GetNewCharInfo()
    {
        return OnCreateCharClass;
    }
    public void setnewChar(bool value)
    {
        newCharcreated = value;
    }
    public bool getCreated()
    {
        return newCharcreated;
    }
    /*
     * Remove Character from system and delete gameobject
     * Used by Classes
     */
    public void DeleteCharacter(string name)
    {
        Characters.Remove(name);
        GameObject obj = GameObject.Find(name);
        DestroyImmediate(obj);
    }
    /*
     * Return the name of all characters in the system
     */
    public HashSet<string> GetAllCharacters()
    {
        return Characters;
    }
    public void SetCharList(HashSet<string> newlist)
    {
        Characters = new HashSet<string>(newlist);
    }
    /*
     * Save the current status of AI_Manager system
     */
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/AIManager" + newScene.name + ".dat");
        ManagerData tempdata = new ManagerData();
        tempdata.Classlist = Classlist;
        tempdata.Characters = Characters;
        tempdata.Eventlist = Eventlist;
        tempdata.Eventnames = Eventnames;
        tempdata.PersonalityTraits = PersonalityTraits;
        tempdata.AvailableIDs = AvailableIDs;
        tempdata.CharacterSize = CharacterSize;
        bf.Serialize(file, tempdata);
        file.Close();
    }
    [Serializable]
    /*
     * Data structure used to store the data of AI_Manager.
     */
    public class ManagerData
    {
        public Dictionary<string, CharacterClass> Classlist;
        public HashSet<string> Characters;
        public HashSet<string> Eventnames;
        public List<AIEvents> Eventlist;
        public IList<string> PersonalityTraits;
        public HashSet<int> AvailableIDs;
        public int CharacterSize;
    }



}



using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class EventGroup
{
    public string groupName;
    public int groupID;
    public List<int> encounterIDs;
}

[CreateAssetMenu(fileName = "NewEventGroupData", menuName = "CJH/Event Group Data")]
public class EventGroupData : ScriptableObject
{
    public List<EventGroup> eventGroups;

    public List<int> GetEncounterIDsByGroupID(int id)
    {
        var group = eventGroups.FirstOrDefault(g => g.groupID == id);
        return group?.encounterIDs ?? new List<int>();
    }
}


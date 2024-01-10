using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public bool isQuestLine;
    public string questLineName;
    public Quest nextQuest;
    public ItemInfo[] itemsNeeded;

    public enum QuestType
    {
        INVENTORY,
        REPAIR,
        PLACE,
    }

    public QuestType questType;
}
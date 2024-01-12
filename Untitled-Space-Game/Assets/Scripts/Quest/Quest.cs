using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string questInfo;

    public bool isQuestLine;
    public string questLineName;

    public Quest nextQuest;

    public ItemInfo[] itemsNeeded;
    public GameObject[] machinesNeeded;

    public Recipe[] recipesToUnlock;

    public enum QuestType
    {
        INVENTORY,
        REPAIR,
        PLACE,
    }

    public QuestType questType;
}
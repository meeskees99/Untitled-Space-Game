using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    public int questId;

    public string questName;
    public string questInfo;

    public bool isQuestLine;
    public string questLineName;

    public Quest nextQuest;

    public ItemInfo[] itemsNeeded;
    public GameObject[] machinesNeeded;

    // Add thing for what repair the ship will get

    public Recipe[] recipesToUnlock;
    public Item[] itemsToGet;

    public enum QuestType
    {
        INVENTORY,
        REPAIR,
        PLACE,
    }

    public QuestType questType;


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public bool isQuestLine;
    public Quest nextQuest;
    public ItemInfo[] itemsNeeded;
}
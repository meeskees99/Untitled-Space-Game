using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] Quest[] _quests;
    [SerializeField] int _questsIndex;
    [SerializeField] Quest _currentQuest;
    [SerializeField] string _currentQuestName;
    [SerializeField] ItemInfo[] _questRequirements;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    public void StartNewQuest()
    {
        _questsIndex++;
        _currentQuest = _quests[_questsIndex];
        _currentQuestName = _currentQuest.questName;
        _questRequirements = _currentQuest.itemsNeeded;
    }

    private void Update()
    {
        switch (_currentQuest.questType)
        {
            case Quest.QuestType.PLACE:
                {
                    break;
                }
            case Quest.QuestType.INVENTORY:
                {
                    CheckInventory();
                    break;
                }
            case Quest.QuestType.REPAIR:
                {
                    CheckRepair();
                    break;
                }
        }
    }

    public void CheckPlace(GameObject machineToLookFor)
    {
        if (machineToLookFor == _currentQuest.machinesNeeded)
        {
            StartNewQuest();
        }
    }

    public bool CheckInventory()
    {
        for (int i = 0; i < _questRequirements.Length; i++)
        {
            for (int j = 0; j < InventoryManager.Instance.itemsInInventory.Count; j++)
            {
                if (_questRequirements[i].item == InventoryManager.Instance.itemsInInventory[j].item)
                {
                    if (_questRequirements[i].amount <= InventoryManager.Instance.itemsInInventory[j].amount)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void CheckRepair()
    {

    }
}

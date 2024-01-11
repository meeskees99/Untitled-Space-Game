using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Quest Info")]
    public static QuestManager Instance;

    [SerializeField] Quest _startQuest;

    [SerializeField] Quest _currentQuest;

    [SerializeField] string _currentQuestName;
    [SerializeField] string _currentQuestInfo;

    [SerializeField] ItemInfo[] _questItemRequirements;
    [SerializeField] GameObject[] _questMachineRequirements;

    [Header("UI")]
    [SerializeField] TMP_Text _questNameTxt;
    [SerializeField] TMP_Text _questInfoTxt;

    [SerializeField] Transform _itemParent;
    [SerializeField] GameObject _itemPrefab;

    [SerializeField] MachinePlacement _machinePlacement;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        _machinePlacement = FindObjectOfType<MachinePlacement>();

        _currentQuest = _startQuest;

        UpdateQuest();
        UpdateItems();
    }

    public void StartNewQuest()
    {
        _currentQuest = _currentQuest.nextQuest;

        UpdateQuest();
        UpdateItems();
    }

    public void UpdateQuest()
    {
        _currentQuestName = _currentQuest.questName;
        _currentQuestInfo = _currentQuest.questInfo;

        _questItemRequirements = _currentQuest.itemsNeeded;
        _questMachineRequirements = _currentQuest.machinesNeeded;

        _questNameTxt.text = _currentQuestName;
        _questInfoTxt.text = _currentQuestInfo;
    }

    public void UpdateItems()
    {
        for (int i = 0; i < _itemParent.childCount; i++)
        {
            Destroy(_itemParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < _questItemRequirements.Length; i++)
        {
            GameObject temp = Instantiate(_itemPrefab, _itemParent);
            temp.GetComponent<Image>().sprite = _questItemRequirements[i].item.image;
            temp.GetComponent<Image>().enabled = true;
            temp.transform.GetComponentInChildren<TMP_Text>().text = _questItemRequirements[i].amount.ToString();
            temp.transform.GetComponentInChildren<TMP_Text>().enabled = true;
        }

    }

    private void Update()
    {
        switch (_currentQuest.questType)
        {
            case Quest.QuestType.PLACE:
                {
                    _machinePlacement._machineQuest = true;
                    break;
                }
            case Quest.QuestType.INVENTORY:
                {
                    _machinePlacement._machineQuest = false;

                    if (CheckInventory())
                    {
                        StartNewQuest();
                        Debug.LogWarning("Inventory Check was true");
                    }
                    break;
                }
            case Quest.QuestType.REPAIR:
                {
                    _machinePlacement._machineQuest = false;
                    if (CheckRepair())
                    {
                        StartNewQuest();
                    }
                    break;
                }
        }
    }

    public void CheckPlace()
    {
        GameObject[] placedMachines = GameObject.FindGameObjectsWithTag("Machine");

        bool[] machinesDone = new bool[_questMachineRequirements.Length];
        for (int i = 0; i < placedMachines.Length; i++)
        {
            for (int j = 0; j < _questMachineRequirements.Length; j++)
            {
                if (placedMachines[i].name == _questMachineRequirements[j].name)
                {
                    machinesDone[j] = true;
                    continue;
                }
            }
        }

        for (int i = 0; i < machinesDone.Length; i++)
        {
            if (machinesDone[i] == false)
            {
                return;
            }
        }

        StartNewQuest();
    }

    public bool CheckInventory()
    {
        for (int i = 0; i < _questItemRequirements.Length; i++)
        {
            for (int j = 0; j < InventoryManager.Instance.itemsInInventory.Count; j++)
            {
                if (_questItemRequirements[i].item == InventoryManager.Instance.itemsInInventory[j].item)
                {
                    if (InventoryManager.Instance.itemsInInventory[j].amount >= _questItemRequirements[i].amount)
                    {
                        continue;
                    }
                    else
                    {
                        Debug.LogWarning("Returned False On InventoryCheck");
                        return false;
                    }
                }
            }
        }
        Debug.LogWarning("Returned True On InventoryCheck");
        return true;
    }

    public bool CheckRepair()
    {
        return false;
    }
}

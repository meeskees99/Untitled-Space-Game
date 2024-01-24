using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    [Header("Quest Info")]
    public static QuestManager Instance;

    [SerializeField] Quest[] _allQuests;

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

    [SerializeField] bool _shipRepairQuest;

    [SerializeField] SkinnedMeshRenderer _shipRenderer;



    [SerializeField] Transform _inventorySlotParent;
    [SerializeField] GameObject _inventorySlotPrefab;

    [SerializeField] List<InventorySlot> _shipRepairSubmitSlots = new();

    bool[] _currentRepairProgress;

    int[] _shipStateAmount = new int[5];

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        _machinePlacement = FindObjectOfType<MachinePlacement>();
        _currentQuest = _startQuest;

        UpdateQuest();
        UpdateItems();
        UpdateQuestType();
        UpdateRepairSlots();
    }

    public void StartNewQuest()
    {
        UpdateQuest();
        UpdateItems();
        UpdateQuestType();
        UpdateRepairSlots();

        CheckPlace();
        CheckInventory();
    }

    public void EndQuest()
    {
        InventoryManager.Instance.canCheckInventoryQuest = false;
        _machinePlacement._machineQuest = false;
        _shipRepairQuest = false;

        if (_currentQuest == null)
        {
            Debug.LogError("No Active Quest On EndQuest");
            return;
        }

        if (_currentQuest.recipesToUnlock != null)
        {
            for (int i = 0; i < _currentQuest.recipesToUnlock.Length; i++)
            {
                CraftingManager.Instance.AddRecipe(_currentQuest.recipesToUnlock[i]);
            }
        }

        if (_currentQuest.repairIndex != null)
        {
            for (int i = 0; i < _currentQuest.repairIndex.Length; i++)
            {
                _shipRenderer.SetBlendShapeWeight(_currentQuest.repairIndex[i], _currentQuest.repairAmount[i]);
                _shipStateAmount[i] = _currentQuest.repairAmount[i];
            }
        }

        if (_currentQuest.nextQuest != null)
        {
            _currentQuest = _currentQuest.nextQuest;
        }
        else
        {
            Debug.Log("No more Quests");
            return;
        }

        StartNewQuest();
    }

    public void UpdateQuest()
    {
        _currentQuestName = _currentQuest.questName;
        _currentQuestInfo = _currentQuest.questInfo;

        _questItemRequirements = _currentQuest.itemsNeeded;
        _questMachineRequirements = _currentQuest.machinesNeeded;

        _questNameTxt.text = _currentQuestName;
        _questInfoTxt.text = _currentQuestInfo;

        _currentRepairProgress = new bool[_questItemRequirements.Length];
    }

    public void UpdateItems()
    {
        for (int i = _itemParent.childCount; i > 0; i--)
        {
            Destroy(_itemParent.GetChild(i - 1).gameObject);
            // Debug.Log($"Destroyed item {_itemParent.GetChild(i - 1).GetComponent<Image>().sprite.name} (total: {i})");
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

    public void UpdateRepairSlots()
    {
        if (!_shipRepairQuest)
        {
            // Debug.Log("No Ship Repair Quest Active");
            return;
        }

        _shipRepairSubmitSlots.Clear();

        for (int i = _inventorySlotParent.childCount; i > 0; i--)
        {
            Destroy(_inventorySlotParent.GetChild(i - 1).gameObject);
            Debug.Log($"Destroyed item {_inventorySlotParent.GetChild(i - 1).gameObject.name} (total: {i})");
        }

        for (int i = 0; i < _questItemRequirements.Length; i++)
        {
            GameObject temp = Instantiate(_inventorySlotPrefab, _inventorySlotParent);
            _shipRepairSubmitSlots.Add(temp.GetComponent<InventorySlot>());
        }
    }

    private void UpdateQuestType()
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
                    InventoryManager.Instance.canCheckInventoryQuest = true;
                    break;
                }
            case Quest.QuestType.REPAIR:
                {
                    _shipRepairQuest = true;
                    break;
                }
        }
    }

    public void CheckPlace()
    {
        if (!_machinePlacement._machineQuest)
        {
            return;
        }

        if (!_questMachineRequirements.Any())
        {
            Debug.Log("did not find any machine requirements");
            return;
        }

        GameObject[] placedMachines = GameObject.FindGameObjectsWithTag("Machine");

        bool[] machinesDone = new bool[_questMachineRequirements.Length];

        for (int i = 0; i < placedMachines.Length; i++)
        {
            for (int j = 0; j < _questMachineRequirements.Length; j++)
            {
                if (placedMachines[i].GetComponent<SmeltingMachine>() && _questMachineRequirements[j].GetComponent<SmeltingMachine>())
                {
                    machinesDone[j] = true;
                    continue;
                }
                else if (placedMachines[i].GetComponent<DiggingMachine>() && _questMachineRequirements[j].GetComponent<DiggingMachine>())
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

        Debug.Log("Check place was true");
        EndQuest();
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
        EndQuest();
        return true;
    }

    public void SubmitQuestBtn()
    {
        if (_shipRepairQuest)
        {
            Debug.Log("Submit quest item");
            for (int i = 0; i < _questItemRequirements.Length; i++)
            {
                if (_currentRepairProgress[i] == true)
                {
                    Debug.Log($"Quest {i + 1} Was Already Completed");
                    continue;
                }
                for (int j = 0; j < _shipRepairSubmitSlots.Count; j++)
                {
                    if (_shipRepairSubmitSlots[j].GetInventoryItem() && _questItemRequirements[i].item == _shipRepairSubmitSlots[j].GetInventoryItem().item)
                    {
                        if (_questItemRequirements[i].amount == _shipRepairSubmitSlots[j].GetInventoryItem().count)
                        {
                            Debug.Log("Had Exactly Enough Items And Destroyed Item");
                            Destroy(_shipRepairSubmitSlots[j].GetInventoryItem().gameObject);
                            // Destroy(_shipRepairSubmitSlots[j].gameObject);
                            _currentRepairProgress[i] = true;
                            continue;
                        }
                        else if (_questItemRequirements[i].amount < _shipRepairSubmitSlots[j].GetInventoryItem().count)
                        {
                            Debug.Log("Had Exactly Enough Items And Destroyed Item");
                            _shipRepairSubmitSlots[j].GetInventoryItem().count -= _questItemRequirements[i].amount;
                            _shipRepairSubmitSlots[j].GetInventoryItem().RefreshCount();
                            _currentRepairProgress[i] = true;
                            continue;
                        }
                        else
                        {
                            Debug.Log($"Did not have enough {_questItemRequirements[i].item.name}");
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < _currentRepairProgress.Length; i++)
            {
                if (_currentRepairProgress[i] == false)
                {
                    Debug.Log("Still Needs More Items To Complete Quest");
                    return;
                }
            }
            Debug.Log("all items have been submitted");
            StartNewQuest();
        }
    }

    public void LoadData(GameData data)
    {
        if (!_allQuests.Any())
        {
            Debug.Log("has no Quests");
            return;
        }

        for (int i = 0; i < _allQuests.Length; i++)
        {
            if (_allQuests[i].questId == data.currentQuestId)
            {
                Debug.Log($"quest id becomes {_allQuests[i].questId}");
                _startQuest = _allQuests[i];
            }
        }
        _shipStateAmount = data.shipState;
    }


    public void SaveData(GameData data)
    {
        Debug.Log($"current quest id {_currentQuest.questId}");
        data.currentQuestId = _currentQuest.questId;

        data.shipState = _shipStateAmount;
    }
}

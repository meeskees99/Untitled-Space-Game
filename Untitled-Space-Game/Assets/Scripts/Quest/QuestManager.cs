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

    [SerializeField] bool _canSubmitQuest;

    [SerializeField] SkinnedMeshRenderer shipRenderer;

    int[] _shipStateAmount = new int[5];

    private void Awake()
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
    }

    public void StartNewQuest()
    {
        UpdateQuest();
        UpdateItems();
    }

    public void EndQuest()
    {
        if (_currentQuest == null)
        {
            Debug.LogError("No Active Quest On EndQuest");
            return;
        }

        for (int i = 0; i < _currentQuest.recipesToUnlock.Length; i++)
        {
            CraftingManager.Instance.AddRecipe(_currentQuest.recipesToUnlock[i]);
        }

        for (int i = 0; i < _currentQuest.repairIndex.Length; i++)
        {
            shipRenderer.SetBlendShapeWeight(_currentQuest.repairIndex[i], _currentQuest.repairAmount[i]);
            _shipStateAmount[i] = _currentQuest.repairAmount[i];
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


    }

    public void UpdateItems()
    {
        for (int i = _itemParent.childCount; i > 0; i--)
        {
            Destroy(_itemParent.GetChild(i - 1).gameObject);
            Debug.Log($"Destroyed item {_itemParent.GetChild(i).GetComponent<Image>().sprite.name} (total: {i})");
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
        if (_currentQuest == null)
        {
            Debug.LogError("No Active Quest");
            return;
        }
        switch (_currentQuest.questType)
        {
            case Quest.QuestType.PLACE:
                {
                    _machinePlacement._machineQuest = true;
                    _canSubmitQuest = false;
                    break;
                }
            case Quest.QuestType.INVENTORY:
                {
                    _machinePlacement._machineQuest = false;
                    _canSubmitQuest = false;

                    if (CheckInventory())
                    {

                        EndQuest();
                        Debug.LogWarning("Inventory Check was true");
                    }
                    break;
                }
            case Quest.QuestType.REPAIR:
                {
                    _machinePlacement._machineQuest = false;
                    _canSubmitQuest = true;
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

        for (int i = 0; i < _currentQuest.recipesToUnlock.Length; i++)
        {
            CraftingManager.Instance.AddRecipe(_currentQuest.recipesToUnlock[i]);
        }
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
        return true;
    }

    public void SubmitQuestBtn()
    {
        if (_canSubmitQuest)
        {
            Debug.Log("Submit quest item");
            for (int i = 0; i < _questItemRequirements.Length; i++)
            {
                for (int j = 0; j < InventoryManager.Instance.itemsInInventory.Count; j++)
                {
                    if (_questItemRequirements[i].item == InventoryManager.Instance.itemsInInventory[j].item)
                    {
                        InventoryManager.Instance.UseItem(_questItemRequirements[i].item.itemID, _questItemRequirements[i].amount);
                        for (int y = 0; y < _currentQuest.recipesToUnlock.Length; y++)
                        {
                            CraftingManager.Instance.AddRecipe(_currentQuest.recipesToUnlock[y]);
                        }
                        EndQuest();
                    }
                }
            }
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

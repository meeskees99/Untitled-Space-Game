using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    [SerializeField] int _gameDifficulty = 1;

    [Header("Stats")]
    public static float Health;
    public static float Oxygen;
    [SerializeField] float _currentHealth;
    [SerializeField] float _currentOxygen;
    public static bool IsAlive;

    [Header("Setup")]
    [SerializeField] Transform _spawnPoint;

    [Header("Difficulty Settings")]

    [Header("Classic")]
    [SerializeField] float _classicMaxHealth = 100;
    [SerializeField] float _classicMinOxygenAmountForRegen = 10f;
    [SerializeField] float _classicSsufficationRate = 0.4f;
    [SerializeField] float _classicHealthRegenerationRate = 0.4f;

    [SerializeField] float _classicMaxOxygen = 100f;
    [SerializeField] float _classicOxygenDepletionRate = 6f;
    [SerializeField] float _classicOxygenRegenerationRate = 4.5f;
    [Header("Hardcore")]
    [SerializeField] float _hardcoreMaxHealth = 80;
    [SerializeField] float _hardcoreMinOxygenAmountForRegen = 60f;
    [SerializeField] float _hardcoreSsufficationRate = 8f;
    [SerializeField] float _hardcoreHealthRegenerationRate = 2f;

    [SerializeField] float _hardcoreMaxOxygen = 60.8f;
    [SerializeField] float _hardcoreOxygenDepletionRate = 8f;
    [SerializeField] float _hardcoreOxygenRegenerationRate = 2.5f;

    [Header("Health Base")]
    [SerializeField] float _maxHealth = 100;
    [SerializeField] float _minOxygenAmountForRegen = 10f;
    [SerializeField] float _sufficationRate = 0.4f;
    [SerializeField] float _healthRegenerationRate = 0.4f;

    [Header("Oxygen Base")]
    [SerializeField] float _maxOxygen = 50f;
    [SerializeField] float _oxygenDepletionRate = 0.5f;
    [SerializeField] float _oxygenRegenerationRate = 0.7f;

    [Header("Attacking")]
    [SerializeField] float _attackDamage = 0.2f;
    public float AttackDamage { get { return _attackDamage; } private set { } }

    [SerializeField] float _attackSpeed = 0.2f;
    public float AttackSpeed { get { return _attackSpeed; } private set { } }

    [Header("UI")]
    [SerializeField] Transform _heartsHolder;
    [SerializeField] Sprite _heartFilledImage;
    [SerializeField] Sprite _heartDepletedImage;


    [Header("Action Bools")]
    public bool recievingOxygen;
    public bool healing;

    bool _hasLoadData;

    void Start()
    {
        if (FindAnyObjectByType<DifficultySetting>().gameDifficulty == -1)
        {
            _gameDifficulty = 1;
        }

        switch (_gameDifficulty)
        {
            case (0):
                _maxHealth = _classicMaxHealth;
                _minOxygenAmountForRegen = _classicMinOxygenAmountForRegen;
                _sufficationRate = _classicSsufficationRate;
                _healthRegenerationRate = _classicHealthRegenerationRate;

                _maxOxygen = _classicMaxOxygen;
                _oxygenDepletionRate = _classicOxygenDepletionRate;
                _oxygenRegenerationRate = _classicOxygenRegenerationRate;
                break;
            case (1):
                return;
            case (2):
                _maxHealth = _hardcoreMaxHealth;
                _minOxygenAmountForRegen = _hardcoreMinOxygenAmountForRegen;
                _sufficationRate = _hardcoreSsufficationRate;
                _healthRegenerationRate = _hardcoreHealthRegenerationRate;

                _maxOxygen = _hardcoreMaxOxygen;
                _oxygenDepletionRate = _hardcoreOxygenDepletionRate;
                _oxygenRegenerationRate = _hardcoreOxygenRegenerationRate;
                break;
        }

        if (!_hasLoadData)
        {
            ResetStats();
        }
    }

    void Update()
    {
        if (!IsAlive || _gameDifficulty == 1)
        {
            return;
        }

        HandleOxygen();
        HandeHealth();
    }

    void HandleOxygen()
    {
        _currentOxygen = Oxygen;
        if (!recievingOxygen)
        {
            if (Oxygen > 0)
            {
                Oxygen -= _oxygenDepletionRate * Time.deltaTime;
            }
            else
            {
                Oxygen = 0;
            }
        }
        else
        {
            if (Oxygen < _maxOxygen)
            {
                Oxygen += _oxygenRegenerationRate * Time.deltaTime;
            }
            else
            {
                Oxygen = _maxOxygen;
            }
        }
    }

    void HandeHealth()
    {
        _currentHealth = Health;

        if (_currentHealth < _maxHealth)
        {
            _heartsHolder.GetChild(2).GetComponent<Image>().sprite = _heartDepletedImage;
            if (_currentHealth <= _maxHealth / 3 * 2)
            {
                _heartsHolder.GetChild(1).GetComponent<Image>().sprite = _heartDepletedImage;
                if (_currentHealth <= _maxHealth / 3)
                {
                    _heartsHolder.GetChild(0).GetComponent<Image>().sprite = _heartDepletedImage;

                }
                else
                {
                    _heartsHolder.GetChild(0).GetComponent<Image>().sprite = _heartFilledImage;
                }
            }
            else
            {
                _heartsHolder.GetChild(1).GetComponent<Image>().sprite = _heartFilledImage;
            }
        }
        else
        {
            _heartsHolder.GetChild(2).GetComponent<Image>().sprite = _heartFilledImage;
        }

        if (Health <= 0 && IsAlive)
        {
            Health = 0;
            Die();
        }
        else if (Oxygen > _minOxygenAmountForRegen)
        {
            if (Health < _maxHealth)
            {
                Health += _healthRegenerationRate * Time.deltaTime;
                healing = true;
            }
            else
            {
                Health = _maxHealth;
                healing = false;
            }
        }
        else if (Oxygen <= 0)
        {
            healing = false;
            if (Health > 0)
            {
                Health -= _sufficationRate * Time.deltaTime;
            }
        }
    }

    public void TakeDamage(float dmgToDo)
    {
        if (dmgToDo > Health)
        {
            float overflow = dmgToDo - Health;
            dmgToDo -= overflow;
        }
        Health -= dmgToDo;
        if (Health < 0)
        {
            Health = 0;
        }
        Debug.Log($"Player Took {dmgToDo} damage!");
    }

    void Die()
    {
        Debug.Log("You Died!");
        IsAlive = false;
        InGameUIManager.Instance.Die(_gameDifficulty);
        InventoryManager.Instance.DropAllItems();
    }

    public void ResetStats()
    {
        Health = _maxHealth;
        Oxygen = _maxOxygen;
        IsAlive = true;
        transform.position = _spawnPoint.position;
        transform.rotation = Quaternion.identity;
        Debug.Log("Reset Player Stats");
    }

    public void LoadData(GameData data)
    {
        _gameDifficulty = data.gameDifficulty;
        if (data.playerHealth == -1)
        {
            _hasLoadData = false;

            transform.position = _spawnPoint.position;
            transform.rotation = _spawnPoint.rotation;

            ResetStats();
            return;
        }
        _hasLoadData = true;


        Health = data.playerHealth;
        Oxygen = data.playerOxygen;

        transform.position = data.playerPosition;
        transform.rotation = data.playerRotation;

    }

    public void SaveData(GameData data)
    {
        data.gameDifficulty = _gameDifficulty;
        data.playerHealth = Health;
        data.playerOxygen = Oxygen;

        data.playerPosition = transform.position;
        data.playerRotation = transform.rotation;
    }
}
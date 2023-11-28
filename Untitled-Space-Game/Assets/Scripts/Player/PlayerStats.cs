using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public static float Health;
    public static float Oxygen;
    [SerializeField] float _currentHealth;
    [SerializeField] float _currentOxygen;


    [Header("Health Settings")]
    [SerializeField] float _maxHealth = 100;
    [SerializeField] float _minOxygenAmountForRegen = 10f;
    [SerializeField] float _sufficationRate = 0.4f;
    [SerializeField] float _healthRegenerationRate = 0.4f;

    [Header("Oxygen Settings")]
    [SerializeField] float _maxOxygen = 50f;
    [SerializeField] float _oxygenDepletionRate = 0.5f;
    [SerializeField] float _oxygenRegenerationRate = 0.7f;

    [Header("Action Bools")]
    public bool recievingOxygen;
    public bool healing;

    void Update()
    {
        HandleOxygen();
        HandeHealth();
    }

    void HandleOxygen()
    {
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
        if (Oxygen > _minOxygenAmountForRegen)
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
            Health -= _sufficationRate * Time.deltaTime;
        }
    }
}

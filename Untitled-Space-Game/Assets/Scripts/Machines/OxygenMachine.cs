using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenMachine : MonoBehaviour, IDataPersistence
{
    [SerializeField] float _startRange = 15f;
    [SerializeField] float _range = 15f;

    public float Range { get { return _range; } set { _range = value; } }


    PlayerStats _playerStats;
    // Start is called before the first frame update
    void Start()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _range);

        foreach (var collider in colliders)
        {
            if (collider.GetComponentInParent<PlayerStats>())
            {
                _playerStats.recievingOxygen = true;
                return;
            }
        }
        _playerStats.recievingOxygen = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    public void LoadData(GameData data)
    {
        if (data.oxygenRange > 0)
        {
            _range = data.oxygenRange;
        }
        else
        {
            _range = _startRange;
        }
    }

    public void SaveData(GameData data)
    {
        data.oxygenRange = _range;
    }
}

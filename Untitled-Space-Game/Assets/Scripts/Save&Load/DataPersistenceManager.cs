using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] string _fileName;
    [SerializeField] bool _useEncryption;

    public static DataPersistenceManager instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _dataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log(instance);
            Debug.LogError("Found a Second instance");
        }
        instance = this;
    }

    private void Start()
    {
        this._dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
        this._dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this._gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO - load any saved data from a file using the data handler
        this._gameData = _dataHandler.Load();

        if (this._gameData == null)
        {
            Debug.Log("No game data was found starting new game");
            NewGame();
        }

        // push the loaded data to all other script that need it
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref _gameData);
        }

        // save that data to a file using the file data handler
        _dataHandler.Save(_gameData);
    }

    public void SaveBtn()
    {
        SaveGame();
    }

    public void LoadBtn()
    {
        LoadGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

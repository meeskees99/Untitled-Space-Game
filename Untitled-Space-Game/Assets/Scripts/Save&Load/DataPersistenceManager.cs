using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool _initializeDataIfNull;

    [Header("File Storage Config")]
    [SerializeField] string _fileName;
    [SerializeField] bool _useEncryption;

    [SerializeField] string _selectedProfileId = "";

    public static DataPersistenceManager instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _dataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log(instance);
            Debug.LogError("Found a Second instance, Destroying new instance");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this._dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);

        this._selectedProfileId = _dataHandler.GetMostRecentlyUpdatedProfileId();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // SceneManager.sceneUnloaded += OnSceneUnLoaded;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this._dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    // public void OnSceneUnLoaded(Scene scene)
    // {
    //     SaveGame();
    // }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this._selectedProfileId = newProfileId;

        LoadGame();
    }

    public void NewGame()
    {
        this._gameData = new GameData();

        SaveGame();
    }

    public void LoadGame()
    {
        // TODO - load any saved data from a file using the data handler
        this._gameData = _dataHandler.Load(_selectedProfileId);

        if (this._gameData == null && _initializeDataIfNull)
        {
            NewGame();
        }

        if (this._gameData == null)
        {
            Debug.Log("No game data was found. A new game needs to be started before it can be loaded");
            return;
        }

        // push the loaded data to all other script that need it
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        if (this._gameData == null)
        {
            Debug.LogWarning("No data was found. A new game must be started before data can be saved");
            return;
        }

        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(_gameData);
        }

        _gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the file data handler
        _dataHandler.Save(_gameData, _selectedProfileId);

        Debug.Log("Save Game");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
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

    public bool HasGameData()
    {
        return _gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return _dataHandler.LoadAllProfiles();
    }

    public List<string> GetAllProfileIds()
    {
        return _dataHandler.LoadAllProfileIds();
    }
}

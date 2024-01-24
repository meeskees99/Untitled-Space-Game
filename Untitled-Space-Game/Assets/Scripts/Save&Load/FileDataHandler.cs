using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor.Rendering;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private bool _useEncryption;

    private readonly string encryptionCodeWord = "egassem sdrawkcab";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
        this._useEncryption = useEncryption;
    }

    public GameData Load(string profileId)
    {
        if (profileId == null)
        {
            return null;
        }

        string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Delete(string profileId)
    {
        string filePath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        string folderPath = Path.Combine(_dataDirPath, profileId);

        Debug.Log($" Delete save: {profileId}\nPath: {filePath}");
        Debug.Log($" Delete save: {profileId}\nPath: {folderPath}");

        File.Delete(filePath);
        Directory.Delete(folderPath);
    }

    public void Save(GameData data, string profileId)
    {
        if (profileId == null)
        {
            return;
        }

        string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId);
                continue;
            }

            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. profileID: " + profileId);
            }
        }

        return profileDictionary;
    }

    public List<string> LoadAllProfileIds()
    {
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
        List<string> profileIds = new List<string>();

        List<string> orderdProfileIds = new List<string>();

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();

        List<GameData> profilesGameDataOrder = new List<GameData>();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId);
                continue;
            }

            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileIds.Add(profileId);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. profileID: " + profileId);
            }

            foreach (KeyValuePair<string, GameData> pair in profilesGameData)
            {
                if (orderdProfileIds.Count == profileIds.Count)
                {
                    break;
                }

                GameData gameData = pair.Value;

                if (!profilesGameDataOrder.Contains(gameData))
                {
                    if (profilesGameDataOrder.Count != 0)
                    {
                        bool inserted = false;
                        foreach (var orderdGamdata in profilesGameDataOrder)
                        {
                            if (!profilesGameDataOrder.Contains(gameData))
                            {
                                if (orderdGamdata == gameData)
                                {
                                    continue;
                                }
                                if (DateTime.FromBinary(gameData.lastUpdated) > DateTime.FromBinary(orderdGamdata.lastUpdated))
                                {
                                    for (int i = 0; i < profilesGameDataOrder.Count; i++)
                                    {
                                        if (profilesGameDataOrder[i] == orderdGamdata)
                                        {
                                            int count = i;
                                            profilesGameDataOrder.Insert(count, gameData);
                                            orderdProfileIds.Insert(count, profileId);
                                            count = profilesGameDataOrder.Count;
                                            inserted = true;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                Debug.Log("cant add: " + gameData.gameDifficulty);
                                break;
                            }
                        }
                        if (!inserted)
                        {
                            profilesGameDataOrder.Add(gameData);
                            orderdProfileIds.Add(profileId);
                        }
                    }
                    else
                    {
                        profilesGameDataOrder.Add(gameData);
                        orderdProfileIds.Add(profileId);
                        break;
                    }
                }
                else
                {
                    Debug.Log("cant add: " + gameData.gameDifficulty);
                }
            }
        }
        return orderdProfileIds;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null)
            {
                continue;
            }

            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }

        return mostRecentProfileId;
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}

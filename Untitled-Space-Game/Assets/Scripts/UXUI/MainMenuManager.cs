using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
// , IDataPersistence
{
    [Header("Continue")]
    #region Continue
    [SerializeField] Button _continueButton;

    #endregion

    [Header("New Game")]
    #region New Game
    [SerializeField] GameObject _newGamePanel;
    public int gameDifficulty;

    [SerializeField] GameObject _classicDifficultySelected;
    [SerializeField] GameObject _creativeDifficultySelected;
    [SerializeField] GameObject _hardcoreDifficultySelected;

    [SerializeField] TMP_InputField _saveFileInputfield;
    [SerializeField] string _saveFileName;
    #endregion

    [Header("Load Game")]
    #region Load Game
    [SerializeField] GameObject _loadGamePanel;

    [SerializeField] List<string> _profileIds = new List<string>();
    [SerializeField] SaveSlot[] _saveSlots;

    #endregion

    [Header("Settings")]
    #region Settings
    [SerializeField] GameObject _settingsPanel;

    [SerializeField] GameObject _audioPanel;
    [SerializeField] GameObject _videoPanel;
    [SerializeField] GameObject _controlsPanel;

    #endregion

    [Header("Credits")]
    #region Credits
    [SerializeField] GameObject _creditsPanel;

    #endregion

    [Header("Quit")]
    #region Quit
    [SerializeField] GameObject _quitPanel;

    #endregion

    // [Header("Menu Buttons")]
    // #region Menu Buttons
    // // TODO - add all buttons

    // #endregion

    private void Awake()
    {
        _profileIds = DataPersistenceManager.instance.GetAllProfileIds();
        _saveSlots = FindObjectsOfType<SaveSlot>();

        print("profile id count: " + _profileIds.Count + " save slot lenght: " + _saveSlots.Length);

        if (_profileIds.Count > 0)
        {
            for (int i = 0; i < _saveSlots.Length; i++)
            {
                if (_profileIds.Count > i)
                {
                    _saveSlots[i].ProfileId = _profileIds[i];
                }
                else
                {
                    Debug.LogError("OOPS");
                }
            }
        }

        LoadSaveFiles();
    }

    public void LoadSaveFiles()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.ProfileId, out profileData);
            Debug.Log("saveSlot profileId: " + saveSlot.ProfileId);
            saveSlot.SetData(profileData);
        }
    }

    private void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            _continueButton.interactable = false;
        }
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in _saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
    }

    // public void LoadData(GameData data)
    // {
    //     this.gameDifficulty = data.gameDifficulty;
    // }

    // public void SaveData(ref GameData data)
    // {
    //     Debug.Log("SAVE MAIN MENU " + this.gameDifficulty);
    //     data.gameDifficulty = this.gameDifficulty;
    // }

    public void ContinueButton()
    {
        // DisableMenuButtons();
        DataPersistenceManager.instance.LoadGame();

        SceneManager.LoadSceneAsync("Inventory");
    }

    #region Load Game

    public void LoadGameButton()
    {
        _loadGamePanel.SetActive(!_loadGamePanel.activeSelf);
        _newGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        // _quitPanel.SetActive(false);
    }

    public void LoadSaveFileButton(SaveSlot saveSlot)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.ProfileId);

        // DataPersistenceManager.instance.NewGame();

        SceneManager.LoadSceneAsync("Inventory");
    }

    #endregion

    #region Settings

    public void SettingsButton()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
        _newGamePanel.SetActive(false);
        _loadGamePanel.SetActive(false);
        _creditsPanel.SetActive(false);
        //_quitPanel.SetActive(false);
    }

    public void AudioPanelButton()
    {
        // _audioPanel.SetActive(!_audioPanel.activeSelf);
        _audioPanel.SetActive(true);
        _videoPanel.SetActive(false);
        _controlsPanel.SetActive(false);
    }

    public void VideoPanelButton()
    {
        // _videoPanel.SetActive(!_videoPanel.activeSelf);
        _videoPanel.SetActive(true);
        _audioPanel.SetActive(false);
        _controlsPanel.SetActive(false);
    }

    public void ControlsPanelButton()
    {
        // _controlsPanel.SetActive(!_controlsPanel.activeSelf);
        _controlsPanel.SetActive(true);
        _videoPanel.SetActive(false);
        _audioPanel.SetActive(false);
    }

    #endregion

    #region Panel Buttons

    #region New Game

    public void NewGameButton()
    {
        _newGamePanel.SetActive(!_newGamePanel.activeSelf);
        _loadGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        // _quitPanel.SetActive(false);
    }

    public void SelectDifficultyButton(int index)
    {
        Debug.Log("SELECT GAME DIFFICULTY " + index);
        gameDifficulty = index;

        if (index == 0)
        {
            _classicDifficultySelected.SetActive(!_classicDifficultySelected.activeSelf);
            _creativeDifficultySelected.SetActive(false);
            _hardcoreDifficultySelected.SetActive(false);
            return;
        }
        if (index == 1)
        {
            _creativeDifficultySelected.SetActive(!_creativeDifficultySelected.activeSelf);
            _classicDifficultySelected.SetActive(false);
            _hardcoreDifficultySelected.SetActive(false);
            return;
        }
        if (index == 2)
        {
            _hardcoreDifficultySelected.SetActive(!_hardcoreDifficultySelected.activeSelf);
            _classicDifficultySelected.SetActive(false);
            _creativeDifficultySelected.SetActive(false);
            return;
        }
    }

    public void SetSaveName()
    {
        _saveFileName = _saveFileInputfield.text;
        // TODO - Change File Name For New Save In Game Save
    }

    public void StartNewGameButton()
    {
        // DisableMenuButtons();

        DataPersistenceManager.instance.ChangeSelectedProfileId(_saveFileName);

        DataPersistenceManager.instance.NewGame();

        SceneManager.LoadSceneAsync("Inventory");
    }


    #endregion

    public void CreditsButton()
    {
        _creditsPanel.SetActive(!_creditsPanel.activeSelf);
        _newGamePanel.SetActive(false);
        _loadGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        // _quitPanel.SetActive(false);
    }

    public void QuitButton()
    {
        _quitPanel.SetActive(!_quitPanel.activeSelf);
        _newGamePanel.SetActive(false);
        _loadGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
    }

    #endregion
}

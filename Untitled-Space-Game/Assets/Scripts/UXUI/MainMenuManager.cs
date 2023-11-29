using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class MainMenuUIManager : MonoBehaviour
// , IDataPersistence
{
    [Header("SceneToLoad")]
    [SerializeField] string _sceneToLoad;

    [Header("Continue")]
    #region Continue
    [SerializeField] Button _continueButton;

    #endregion

    [Header("New Game")]
    #region New Game
    [SerializeField] GameObject _newGamePanel;
    [SerializeField] GameObject _firstNewGameButton;

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
    [SerializeField] GameObject _firstLoadButton;

    [SerializeField] List<string> _profileIds = new List<string>();

    [SerializeField] Transform _saveSlotParent;
    [SerializeField] GameObject _saveSlotPrefab;
    [SerializeField] List<SaveSlot> _saveSlots = new List<SaveSlot>();

    #endregion

    [Header("Settings")]
    #region Settings
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] GameObject _firstSettingsButton;

    [Header("Audio")]
    [SerializeField] GameObject _audioPanel;
    [SerializeField] GameObject _firstAudioButton;

    [Header("Video")]
    [SerializeField] GameObject _videoPanel;
    [SerializeField] GameObject _firstVideoButton;

    [Header("Controls")]
    [SerializeField] GameObject _controlsPanel;
    [SerializeField] GameObject _firstControlsButton;

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

    public void LoadSaveFiles()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.ProfileId, out profileData);
            saveSlot.SetData(profileData);
        }
    }

    private void Start()
    {
        _profileIds = DataPersistenceManager.instance.GetAllProfileIds();

        if (_profileIds.Count > 0)
        {
            for (int i = 0; i < _profileIds.Count; i++)
            {
                _saveSlots.Add(Instantiate(_saveSlotPrefab, _saveSlotParent).GetComponent<SaveSlot>());
            }
            for (int i = 0; i < _saveSlots.Count; i++)
            {
                int index = i;
                _saveSlots[index].ProfileId = _profileIds[index];
                _saveSlots[index].SaveFileButton.onClick.AddListener(delegate { LoadSaveFileButton(_saveSlots[index]); });
            }
        }

        if (_saveSlots.Count != 0)
        {
            _firstLoadButton = _saveSlots[0].gameObject;
        }

        LoadSaveFiles();

        if (!DataPersistenceManager.instance.HasGameData())
        {
            _continueButton.interactable = false;
        }

        for (int i = 0; i < _saveSlots.Count; i++)
        {
            Navigation currentNav = new Navigation();
            currentNav.mode = Navigation.Mode.Explicit;

            if (i == 0)
            {
                if (_saveSlots.Count == 1)
                {
                    currentNav.selectOnUp = _saveSlots[i].SaveFileButton;
                    currentNav.selectOnDown = _saveSlots[i].SaveFileButton;
                    _saveSlots[i].SaveFileButton.navigation = currentNav;
                }
                else
                {
                    currentNav.selectOnUp = _saveSlots[_saveSlots.Count - 1].SaveFileButton;
                    currentNav.selectOnDown = _saveSlots[i + 1].SaveFileButton;
                    _saveSlots[i].SaveFileButton.navigation = currentNav;
                }
            }
            else if (i == _saveSlots.Count - 1)
            {
                currentNav.selectOnUp = _saveSlots[i - 1].SaveFileButton;
                currentNav.selectOnDown = _saveSlots[0].SaveFileButton;
                _saveSlots[i].SaveFileButton.navigation = currentNav;
            }
            else
            {
                currentNav.selectOnUp = _saveSlots[i - 1].SaveFileButton;
                currentNav.selectOnDown = _saveSlots[i + 1].SaveFileButton;
                _saveSlots[i].SaveFileButton.navigation = currentNav;
            }
        }
    }

    // private void DisableMenuButtons()
    // {
    //     foreach (SaveSlot saveSlot in _saveSlots)
    //     {
    //         saveSlot.SetInteractable(false);
    //     }
    // }

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

        SceneManager.LoadSceneAsync(_sceneToLoad);
    }

    #region New Game

    public void NewGameButton()
    {
        Debug.Log("NEW GAME");
        _newGamePanel.SetActive(!_newGamePanel.activeSelf);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstNewGameButton);

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

        SceneManager.LoadSceneAsync(_sceneToLoad);
    }


    #endregion

    #region Load Game

    public void LoadGameButton()
    {
        _loadGamePanel.SetActive(!_loadGamePanel.activeSelf);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstLoadButton);

        _newGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        // _quitPanel.SetActive(false);
    }

    public void LoadSaveFileButton(SaveSlot saveSlot)
    {
        Debug.Log("load save: " + saveSlot.ProfileId);

        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.ProfileId);

        // DataPersistenceManager.instance.NewGame();

        SceneManager.LoadSceneAsync(_sceneToLoad);
    }

    #endregion

    #region Settings

    public void SettingsButton()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSettingsButton);

        _newGamePanel.SetActive(false);
        _loadGamePanel.SetActive(false);
        _creditsPanel.SetActive(false);
        //_quitPanel.SetActive(false);
    }

    public void AudioPanelButton()
    {
        // _audioPanel.SetActive(!_audioPanel.activeSelf);
        _audioPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstAudioButton);

        _videoPanel.SetActive(false);
        _controlsPanel.SetActive(false);
    }

    public void VideoPanelButton()
    {
        // _videoPanel.SetActive(!_videoPanel.activeSelf);
        _videoPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstVideoButton);

        _audioPanel.SetActive(false);
        _controlsPanel.SetActive(false);
    }

    public void ControlsPanelButton()
    {
        // _controlsPanel.SetActive(!_controlsPanel.activeSelf);
        _controlsPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstControlsButton);

        _videoPanel.SetActive(false);
        _audioPanel.SetActive(false);
    }

    #endregion

    #region Panel Buttons

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

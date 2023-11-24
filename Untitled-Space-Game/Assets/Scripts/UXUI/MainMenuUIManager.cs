using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuUIManager : MonoBehaviour
{
    [Header("NewGame")]
    #region NewGamePanel
    [SerializeField] GameObject _newGamePanel;
    public int gameDifficulty;
    [SerializeField] TMP_InputField _saveFileInputfield;
    public string saveFileName;
    #endregion

    [Header("LoadGame")]
    #region LoadGamePanel
    [SerializeField] GameObject _loadGamePanel;

    #endregion

    [Header("Settings")]
    #region SettingsPanel
    [SerializeField] GameObject _settingsPanel;

    [SerializeField] GameObject _audioPanel;
    [SerializeField] GameObject _videoPanel;
    [SerializeField] GameObject _controlsPanel;

    #endregion

    [Header("CreditsPanel")]
    #region CreditsPanel
    [SerializeField] GameObject _creditsPanel;

    #endregion

    [Header("Quit")]
    #region QuitPanel
    [SerializeField] GameObject _quitPanel;

    #endregion

    public void ContinueButton()
    {

    }

    #region New Game

    public void SelectDifficultyButton(int index)
    {
        gameDifficulty = index;
    }

    public void SetSaveName()
    {
        saveFileName = _saveFileInputfield.text;
        // TODO - Change File Name For New Save In Game Save
    }

    public void StartNewGameButton(int index)
    {
        //TODO - Load New Scene With Difficulty Index
    }


    #endregion

    #region Load Game

    public void LoadSaveFileButton()
    {

    }

    #endregion

    #region Settings

    public void AudioPanelButton()
    {

    }

    public void VideoPanelButton()
    {

    }

    public void ControlsPanelButton()
    {

    }

    #endregion

    #region Panel Buttons
    public void NewGameButton()
    {
        _newGamePanel.SetActive(!_newGamePanel.activeSelf);
    }

    public void LoadGameButton()
    {
        _loadGamePanel.SetActive(!_loadGamePanel.activeSelf);
    }

    public void SettingsButton()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }

    public void CreditsButton()
    {
        _creditsPanel.SetActive(!_creditsPanel.activeSelf);
    }

    public void QuitButton()
    {
        _quitPanel.SetActive(!_quitPanel.activeSelf);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelManager : MonoBehaviour
{
    public UIDocument mainMenuScreen;
    public UIDocument settingsScreen;

    public string username {
        get { return _username; }
        set 
        {
            _username = value;
            _usernameLabel.text = value;
        } 
    }

    private string _username;
    private Label _usernameLabel;

    private void OnEnable()
    {
        BindMainMenuButtons();
        BindSettingButtons();
    }

    private void Start()
    {
        GoToMainMenuScreen();
        username = "Narmaroth"; //the username setter takes care of changeing the usernameLabel
    }

    private void BindMainMenuButtons()
    {
        var root = mainMenuScreen.rootVisualElement;
        var startButton = root.Q<Button>("start-button");
        if (startButton != null)
        {
            startButton.clicked += () => Debug.Log("Start Game");
        }

        var settingsButton = root.Q<Button>("setting-button");
        if (settingsButton != null)
        {
            settingsButton.clicked += GoToSettingsScreen;
        }

        var exitButton = root.Q<Button>("exit-button");
        if (exitButton != null)
        {
            exitButton.clicked += () => Application.Quit();
        }

        _usernameLabel = root.Q<Label>("player-username");
    }

    private void BindSettingButtons() 
    {
        var root = settingsScreen.rootVisualElement;
        var returnButton = root.Q<Button>("return-button");
        if (returnButton != null)
        {
            returnButton.clicked += GoToMainMenuScreen;
        }
    }


    private void GoToMainMenuScreen() 
    {
        mainMenuScreen.sortingOrder = 1;
        settingsScreen.sortingOrder = 0;
    }

    private void GoToSettingsScreen()
    {
        mainMenuScreen.sortingOrder = 0;
        settingsScreen.sortingOrder = 1;
    }
}

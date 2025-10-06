using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private List<Button> _menuButtons = new List<Button>();
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            int index = i; // Capture the current value of i
            _menuButtons[i].RegisterCallback<ClickEvent>(evt => OnMenuButtonClicked(evt, _menuButtons[index].name));
        }
    }

    private void OnMenuButtonClicked(ClickEvent evt, string buttonName)
    {
        Debug.Log($"Button '{buttonName}' clicked!");
        _audioSource.Play();

        switch (buttonName)
        {
            case "StartBtn":
                OnStartButtonClicked();
                break;
            case "SettingsBtn":
                OnSettingsButtonClicked();
                break;
            case "ExitBtn":
                OnExitButtonClicked();
                break;
            default:
                Debug.LogWarning($"Unknown button clicked: {buttonName}");
                break;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            int index = i;
            _menuButtons[i].UnregisterCallback<ClickEvent>(evt => OnMenuButtonClicked(evt, _menuButtons[index].name));
        }
    }

    #region Button Callbacks
    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");
        SceneManager.LoadScene("Main Stage");
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked!");
        // Add your settings menu logic here
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion
}

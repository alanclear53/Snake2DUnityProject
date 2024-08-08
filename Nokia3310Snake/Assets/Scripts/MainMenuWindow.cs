using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(StartGame);
        playButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

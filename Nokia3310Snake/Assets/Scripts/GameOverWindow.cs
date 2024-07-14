using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private static GameOverWindow instance;

    private void Awake()
    {
        instance = this;

        transform.Find("retryButton").GetComponent<Button>().onClick.AddListener(TaskOnClick);

        Hide();
    }
    void TaskOnClick()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void ShowStatic()
    {
        instance.Show();
    }
}

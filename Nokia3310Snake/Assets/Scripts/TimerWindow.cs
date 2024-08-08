using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerWindow : MonoBehaviour
{
    private static TimerWindow instance;
    [SerializeField] private Text timerText;

    public delegate void UpdateTimerDelegate(float time);
    public static event UpdateTimerDelegate OnUpdateTimer;

    private void Awake()
    {
        instance = this;

        Hide();
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

    public static void HideStatic()
    {
        instance.Hide();
    }

    public static void UpdateTimerText(float time)
    {
        OnUpdateTimer?.Invoke(time);
    }

    private void OnEnable()
    {
        OnUpdateTimer += UpdateTimer;
    }

    private void OnDisable()
    {
        OnUpdateTimer -= UpdateTimer;
    }

    private void UpdateTimer(float time)
    {
        timerText.text = $"{time:F0}";
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        scoreText.text = GameHandler.GetScore().ToString();
    }
}

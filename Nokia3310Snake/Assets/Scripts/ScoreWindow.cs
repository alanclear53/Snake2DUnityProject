using TMPro;
using UnityEngine;

public class ScoreWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        GameHandler.OnScoreChanged += UpdateScoreText;
    }

    private void OnDisable()
    {
        GameHandler.OnScoreChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}

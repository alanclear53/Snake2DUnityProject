using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;

    private static int score;

    [SerializeField] private Snake snake;
    

    private LevelGrid levelGrid;

    private void Awake()
    {
        instance = this;
        InitializeStatic();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(30, 30);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

    private static void InitializeStatic()
    {
        score = 0;
    }

    public static int GetScore() 
    { 
        return score; 
    }

    public static void AddScore() 
    { 
        score += 10; 
    }

    public static void SnakeDied()
    {
        GameOverWindow.ShowStatic();
    }
}

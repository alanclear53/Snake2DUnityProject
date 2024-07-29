using System;
using UnityEngine;
using static Snake;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;
    private static int score;

    [SerializeField] private Snake snake;
    public static event Action<int> OnScoreChanged;

    private LevelGrid levelGrid;
    private InputManager userInputManager;
    public Food food;

    private float moveIntervalTimer;
    private const float moveIntervalTimerMax = 0.2f;

    private void Awake()
    {
        moveIntervalTimer = moveIntervalTimerMax;
        instance = this;
        InitializeStatic();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(30, 30);
        userInputManager = new InputManager();

        snake.state = Snake.State.Alive;

        food = new Food();
        food.Spawn(levelGrid);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake, food);
    }

    private void Update()
    {
        switch (snake.state)
        {
            case Snake.State.Alive:
                UpdateMoveInterval();
                snake.SetHeadGridMoveDirection(userInputManager.GetDirection());

                if (moveIntervalTimer >= moveIntervalTimerMax)
                {
                    snake.HandleGridMovement();

                    if (levelGrid.cell[snake.headGridPosition.x, snake.headGridPosition.y].IsOccupiedByFood())
                    {
                        food.Destroy();
                        food.Spawn(levelGrid);

                        GameHandler.AddScore();

                        snake.bodySize++;
                        snake.CreateBodyPart();
                    }
                    snake.UpdateBodyParts();

                    foreach (SnakeBodyPart snakeBodyPart in snake.bodyPartList)
                    {
                        Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();

                        if (snake.headGridPosition == snakeBodyPartGridPosition)
                        {
                            snake.state = Snake.State.Dead;
                            GameHandler.HandleSnakeDeath();
                        }
                    }

                    snake.RemoveLastBodyPosition();

                    userInputManager.moveCounter += 1;
                    moveIntervalTimer -= moveIntervalTimerMax;
                }

                break;
            case Snake.State.Dead:
                break;
            default:
                Debug.LogError("Unexpected snake state");
                break;
        }
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
        OnScoreChanged?.Invoke(score);
    }

    public static void HandleSnakeDeath()
    {
        GameOverWindow.ShowStatic();
    }

    public void UpdateMoveInterval()
    {
        moveIntervalTimer += Time.deltaTime;
    }
}
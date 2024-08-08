using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Snake;
using System.Collections;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;
    private static int score;

    [SerializeField] private Snake snake;
    public static event Action<int> OnScoreChanged;

    private LevelGrid levelGrid;
    private InputManager userInputManager;
    private FoodContainer foodContainer;
    public Food food;

    private float moveIntervalTimer;
    private const float moveIntervalTimerMax = 0.2f;
    private float bonusCountdown = 0f;

    private void Awake()
    {
        instance = this;
        levelGrid = new LevelGrid(30, 30);
        userInputManager = new InputManager();
        moveIntervalTimer = moveIntervalTimerMax;
        InitializeStatic();
    }

    private void Start()
    {

        snake.state = Snake.State.Alive;

        foodContainer = new FoodContainer();
        foodContainer.SpawnFood(levelGrid, 1, FoodType.Regular);

        snake.Setup(levelGrid, foodContainer);
        levelGrid.Setup(snake, foodContainer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isGamePaused())
            {
                GameHandler.ResumeGame();
            }
            else
            {
                GameHandler.PauseGame();
            }
        }

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
                        HandleFood();
                    }

                    snake.UpdateBodyParts();

                    if (HasSelfCollided())
                    {
                        snake.state = Snake.State.Dead;
                        GameHandler.HandleSnakeDeath();
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

    private bool HasSelfCollided()
    {
        foreach (SnakeBodyPart snakeBodyPart in snake.bodyPartList)
        {
            Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();

            if (snake.headGridPosition == snakeBodyPartGridPosition)
            {
                return true; 
            }
        }
        return false;
    }

    private IEnumerator CountdownTimer()
    {
        while (bonusCountdown > 0f)
        {
            yield return new WaitForSeconds(1f);
            bonusCountdown -= 1f;
            TimerWindow.UpdateTimerText(bonusCountdown);
        }

        RemoveBonusFood();
        TimerWindow.HideStatic();
    }

    private void HandleFood()
    {
        Food food = foodContainer.GetFoodAtPosition(snake.headGridPosition);
        var chance = UnityEngine.Random.Range(0f, 0.9f);

        Debug.Log($"Food: {food} | FoodContainer: {foodContainer.foodDictionary.Count} items | FoodContainer contents: [{string.Join(", ", foodContainer.foodDictionary.Select(kv => $"{kv.Key} : {kv.Value.GetType().Name}"))}]");

        AddScore(food.GetScore());
        foodContainer.RemoveFood(food);

        if (food is RegularFood)
        {
            foodContainer.SpawnFood(levelGrid, 1, FoodType.Regular);
        }

        if (food is BonusFood)
        {
            StopCoroutine(CountdownTimer());
            TimerWindow.HideStatic();
        }

        if (!foodContainer.HasFood(FoodType.Bonus) && chance <= 0.2 && bonusCountdown <= 0f)
        {
            foodContainer.SpawnFood(levelGrid, 1, FoodType.Bonus);
            bonusCountdown = 10f;
            StartCoroutine(CountdownTimer());
            TimerWindow.ShowStatic();
        }

        snake.bodySize++;
        snake.CreateBodyPart();
    }

    private void RemoveBonusFood()
    {
        foreach (var food in foodContainer.foodDictionary.Values.ToList())
        {
            if (food is BonusFood)
            {
                foodContainer.RemoveFood(food);
                break;
            }
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

    public static void AddScore(int points)
    {
        score += points;
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

    public static void ResumeGame()
    {
        PauseWindow.HideStatic();
        Time.timeScale = 1f;
    }

    public static void PauseGame()
    {
        PauseWindow.ShowStatic();
        Time.timeScale = 0f;
    }

    public static bool isGamePaused()
    {
        return Time.timeScale == 0f;
    }
}
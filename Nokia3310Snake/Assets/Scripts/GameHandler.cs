using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Snake;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;

    private static GameHandler instance;
    private LevelGrid levelGrid;
    private InputManager userInputManager;
    private FoodContainer foodContainer;
    public Food food;

    private static int score;
    public static event Action<int> OnScoreChanged;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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

                IncreaseMoveIntervalTimer();
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
                    DecreaseMoveIntervalTimer();
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
        if (snake.bodyPartList == null) return false;

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

        //Debug.Log($"Food: {food} | FoodContainer: {foodContainer.foodDictionary.Count} items | FoodContainer contents: [{string.Join(", ", foodContainer.foodDictionary.Select(kv => $"{kv.Key} : {kv.Value.GetType().Name}"))}]");

        AddScore(food.GetScore());
        foodContainer.RemoveFood(food);

        switch (food)
        {
            case RegularFood _:
                foodContainer.SpawnFood(levelGrid, 1, FoodType.Regular);
                break;

            case BonusFood _:
                StopCoroutine(CountdownTimer());
                TimerWindow.HideStatic();
                break;
        }

        if (ShouldSpawnBonusFood())
        {
            foodContainer.SpawnFood(levelGrid, 1, FoodType.Bonus);
            bonusCountdown = 10f;
            StartCoroutine(CountdownTimer());
            TimerWindow.ShowStatic();
        }

        snake.bodySize++;
        snake.CreateBodyPart();
    }

    bool ShouldSpawnBonusFood()
    {
        var chance = UnityEngine.Random.Range(0f, 0.9f);

        return !foodContainer.HasFood(FoodType.Bonus)
            && chance <= 0.2
            && bonusCountdown <= 0f;
    }

    private void RemoveBonusFood()
    {
        foreach (var food in foodContainer.foodDictionary.Values.ToList())
        {
            if (food is BonusFood)
            {
                foodContainer.RemoveFood(food);
                levelGrid.cell[food.GetPosition().x, food.GetPosition().y].SetEmpty();
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

    public void IncreaseMoveIntervalTimer()
    {
        moveIntervalTimer += Time.deltaTime;
    }

    public void DecreaseMoveIntervalTimer()
    {
        moveIntervalTimer -= moveIntervalTimerMax;
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
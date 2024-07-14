using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    public Vector2Int oldFoodGridPosition;
    private GameObject foodGameObject;
    private GameObject snakeGameObject;
    private int width;
    private int height;
    private Snake snake;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void Setup(Snake snake)
    {
        this.snake = snake;

        SpawnFood();
    }

    private void SpawnFood()
    {
        if (foodGridPosition != null)
        {
            oldFoodGridPosition = foodGridPosition;
        }

        do
        {
            foodGridPosition = new Vector2Int(Random.Range(1, width-1), Random.Range(1, height-1));
        }   
        while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        Sprite foodSprite = GameAssets.instance.foodSprite;
        foodGameObject.GetComponent<SpriteRenderer>().sprite = foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {

        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            GameHandler.AddScore();
            return true;
        } 
        
        return false;
        
        
    }

    public void SnakeBiteFood()
    {
        snake.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeHeadMouthOpenSprite;
    }

    public bool SnakeNearFood(Vector2Int snakeGridPosition)
    {
        if (Vector2.Distance(snakeGridPosition, foodGridPosition) <= 1f)
        {
            return true;
        }

        return false;
    }

    public Vector2Int ValidateGridPosition (Vector2Int gridPosition)
    {
        if(gridPosition.x < 0)
        {
            gridPosition.x = width - 1;
        }
        if(gridPosition.x > width - 1)
        {
            gridPosition.x = 0;
        }    
        if (gridPosition.y < 0)
        {
            gridPosition.y = height - 1;
        }
        if (gridPosition.y > height - 1)
        {
            gridPosition.y = 0;
        }
        return gridPosition;
    }

    public Vector2Int getSnakeFoodPosition()
    {
        return foodGridPosition;
    }
}

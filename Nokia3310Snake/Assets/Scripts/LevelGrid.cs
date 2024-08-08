using UnityEngine;

public class LevelGrid
{
    public Vector2Int oldFoodGridPosition;
    private GameObject foodGameObject;
    private GameObject snakeGameObject;

    public int width;
    public int height;

    public Cell[,] cell;

    private Snake snake;
    public Food food;
    private FoodContainer foodContainer;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        this.cell = new Cell[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                this.cell[i, j] = new Cell(i, j);
            }
        }
    }

    public void Setup(Snake snake, FoodContainer foodContainer)
    {
        this.snake = snake;
        this.foodContainer = foodContainer;
    }

    public Vector2Int ValidateCrossedBorder(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0) gridPosition.x = width - 1;

        if (gridPosition.x > width - 1) gridPosition.x = 0;

        if (gridPosition.y < 0) gridPosition.y = height - 1;

        if (gridPosition.y > height - 1) gridPosition.y = 0;


        return gridPosition;
    }

    public Vector2Int GetFoodPosition()
    {
        foreach (Food food in foodContainer.GetFoodList())
        {
            return food.GetPosition();
        }
        return Vector2Int.zero;
    }
}


public class Cell
{
    private int X;
    private int Y;
    private bool OccupiedByFood;
    private bool OccupiedBySnake;

    public Cell(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.OccupiedByFood = false;
        this.OccupiedBySnake = false;
    }

    public bool IsOccupiedByFood()
    {
        return this.OccupiedByFood;
    }

    public bool IsOccupiedBySnake()
    {
        return this.OccupiedBySnake;
    }

    public void SetOccupiedByFood()
    {
        this.OccupiedByFood = true;
    }

    public void SetOccupiedBySnake()
    {
        this.OccupiedBySnake = true;
    }

    public void SetOccupiedByFoodAndSnake()
    {
        this.OccupiedByFood = true;
        this.OccupiedBySnake = true;
    }

    public void SetEmpty()
    {
        this.OccupiedByFood = false;
        this.OccupiedBySnake = false;
    }

    public void RemoveFood()
    {
        this.OccupiedByFood = false;
    }

    public void RemoveSnake()
    {
        this.OccupiedBySnake = false;
    }
}

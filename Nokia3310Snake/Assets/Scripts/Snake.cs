using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,  
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }

    private State state;
    private Direction snakeHeadGridMoveDirection;
    private Vector2Int snakeHeadGridPosition;

    private float gridMoveTimer;
    private float gridMoveTimerMax;

    private LevelGrid levelgrid;
    private int snakeBodySize;

    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;


    public void Setup(LevelGrid levelgrid)
    {
        this.levelgrid = levelgrid;
    }

    private void Awake()
    {
        snakeHeadGridPosition = new Vector2Int(15, 15);
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        snakeHeadGridMoveDirection = Direction.Right;

        snakeBodySize = 0;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }

    private void Update()
    { 
        switch(state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (snakeHeadGridMoveDirection != Direction.Down)
            {
                snakeHeadGridMoveDirection = Direction.Up;
            }

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (snakeHeadGridMoveDirection != Direction.Up)
            {
                snakeHeadGridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (snakeHeadGridMoveDirection != Direction.Right)
            {
                snakeHeadGridMoveDirection = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (snakeHeadGridMoveDirection != Direction.Left)
            {
                snakeHeadGridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0) 
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, snakeHeadGridPosition, snakeHeadGridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (snakeHeadGridMoveDirection)
            {
                default:
                    case Direction.Right:   gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                    case Direction.Left:    gridMoveDirectionVector = new Vector2Int(-1, 0); break;   
                    case Direction.Up:      gridMoveDirectionVector = new Vector2Int(0, +1); break;
                    case Direction.Down:    gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            snakeHeadGridPosition += gridMoveDirectionVector;

            snakeHeadGridPosition = levelgrid.ValidateGridPosition(snakeHeadGridPosition);

            bool snakeNearFood = levelgrid.SnakeNearFood(snakeHeadGridPosition);

            if (snakeNearFood)
            {
                levelgrid.SnakeBiteFood();
            }
            else this.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeHeadSprite;

            bool snakeAteFood = levelgrid.TrySnakeEatFood(snakeHeadGridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList) 
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if(snakeHeadGridPosition == snakeBodyPartGridPosition)
                {
                    //gameover
                    state = State.Dead;
                    GameHandler.SnakeDied();
                }
            }

            transform.position = new Vector3(snakeHeadGridPosition.x, snakeHeadGridPosition.y);
            //угол поворота головы змеи
            switch (snakeHeadGridMoveDirection)
            {
                default:
                    case Direction.Right: transform.eulerAngles = new Vector3(0, 0, 0); break;
                    case Direction.Left: transform.eulerAngles = new Vector3(-180, 0, 180); break;
                    case Direction.Up: transform.eulerAngles = new Vector3(0, 0, 90); break;
                    case Direction.Down: transform.eulerAngles = new Vector3(0, 180, -90); break;
            }
        }
        
    }

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);

            if (i < (snakeBodyPartList.Count - 1))
            {
                Vector2Int snakeBodyPartPositionInt = new Vector2Int(Mathf.RoundToInt(snakeBodyPartList[i].transform.position.x), Mathf.RoundToInt(snakeBodyPartList[i].transform.position.y));

                //змея глотает еду
                if (snakeBodyPartPositionInt == levelgrid.oldFoodGridPosition)
                {
                    snakeBodyPartList[i].transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodyEatSprite;
                }
                else
                {
                    snakeBodyPartList[i].transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
                }


            }

        }

    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2 (dir.x, dir.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public List<Vector2Int> GetFullSnakeGridPositionList() 
    { 
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { snakeHeadGridPosition };

        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList) 
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }

        return gridPositionList;
    }

    private class SnakeBodyPart
    {
        private SnakeMovePosition SnakeMovePosition;
        public Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeTailSprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
            //transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) 
        {
            this.SnakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            float y = 0;

            //поворот тела змеи
            switch (SnakeMovePosition.GetDirection())
            {
                default:
                    case Direction.Up:
                        angle = 90;
                        break;
                    case Direction.Down:
                        angle = -90;
                        y = 180;
                        break;
                    case Direction.Left:
                        angle = 0;
                        y = 180;
                        break;
                    case Direction.Right:
                        angle = 0;
                        break;
            }

            transform.eulerAngles = new Vector3(0, y, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return SnakeMovePosition.GetGridPosition();
        }

    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition= previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() 
        { 
            return gridPosition; 
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else 
            {
                return previousSnakeMovePosition.direction;
            }
        }
    }
}

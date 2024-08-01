using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public enum State
    {
        Alive,
        Dead
    }

    public State state;
    private InputManager.Direction headGridMoveDirection;
    public Vector2Int headGridPosition { get; private set; }

    private LevelGrid levelgrid;

    public int bodySize;

    private List<SnakeMovePosition> movePositionList;
    public List<SnakeBodyPart> bodyPartList;

    private Vector2Int startingPosition = new(15, 15);


    public void Setup(LevelGrid levelgrid)
    {
        this.levelgrid = levelgrid;

        headGridPosition = startingPosition;
        bodySize = 0;

        movePositionList = new List<SnakeMovePosition>();
        bodyPartList = new List<SnakeBodyPart>();
    }

    public void HandleGridMovement()
    {
        SnakeMovePosition previousMovePosition = null;
        if (movePositionList.Count > 0) previousMovePosition = movePositionList[0];

        SnakeMovePosition snakeMovePosition = new(previousMovePosition, headGridPosition, headGridMoveDirection);
        movePositionList.Insert(0, snakeMovePosition);

        this.MoveAndRotateHead();

        headGridPosition = levelgrid.ValidateCrossedBorder(headGridPosition);

        levelgrid.cell[headGridPosition.x, headGridPosition.y].SetOccupiedBySnake();

        if (this.IsHeadNearFood(levelgrid.GetFoodPosition()))
        {
            this.OpenMouth();
        }
        else this.CloseMouth();

        transform.position = new Vector3(headGridPosition.x, headGridPosition.y);
    }

    public void RemoveLastBodyPosition()
    {
        if (movePositionList.Count >= bodySize + 1 && bodySize > 0)
        {
            movePositionList.RemoveAt(movePositionList.Count - 1);
            levelgrid.cell[bodyPartList[bodySize - 1].GetGridPosition().x, bodyPartList[bodySize - 1].GetGridPosition().y].SetEmpty();
        }
    }

    public void MoveAndRotateHead()
    {
        Vector2Int gridMoveDirectionVector;
        switch (headGridMoveDirection)
        {
            default:
            case InputManager.Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
            case InputManager.Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
            case InputManager.Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
            case InputManager.Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
        }
        headGridPosition += gridMoveDirectionVector;

        switch (headGridMoveDirection)
        {
            default:
            case InputManager.Direction.Right: transform.eulerAngles = new Vector3(0, 0, 0); break;
            case InputManager.Direction.Left: transform.eulerAngles = new Vector3(-180, 0, 180); break;
            case InputManager.Direction.Up: transform.eulerAngles = new Vector3(0, 0, 90); break;
            case InputManager.Direction.Down: transform.eulerAngles = new Vector3(0, 180, -90); break;
        }
    }

    private void OpenMouth()
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeHeadMouthOpenSprite;
    }

    private void CloseMouth()
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeHeadSprite;
    }

    public bool IsHeadNearFood(Vector2Int foodPosition)
    {
        if (Vector2.Distance(headGridPosition, foodPosition) <= 1f)
        {
            return true;
        }

        return false;
    }
    public void SetHeadGridMoveDirection(InputManager.Direction direction)
    {
        this.headGridMoveDirection = direction;
    }

    public void CreateBodyPart()
    {
        bodyPartList.Add(new SnakeBodyPart(bodyPartList.Count));
    }

    public void UpdateBodyParts()
    {
        for (int i = 0; i < bodyPartList.Count; i++)
        {
            bodyPartList[i].SetMovePosition(movePositionList[i]);

            if (i < (bodyPartList.Count - 1))
            {
                FoodDigestion(bodyPartList[i]);
            }
        }
    }

    private void FoodDigestion(SnakeBodyPart bodyPart)
    {
        if (bodyPart == null) return;

        Vector2Int snakeBodyPartPositionInt = new Vector2Int(Mathf.RoundToInt(bodyPart.GetGridPosition().x), Mathf.RoundToInt(bodyPart.GetGridPosition().y));

        if (levelgrid.cell[snakeBodyPartPositionInt.x, snakeBodyPartPositionInt.y].IsOccupiedByFood()
            && levelgrid.cell[snakeBodyPartPositionInt.x, snakeBodyPartPositionInt.y].IsOccupiedBySnake())
        {
            bodyPart.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodyFullStomachSprite;
        }
        else
        {
            bodyPart.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
        }
    }

    public List<Vector2Int> GetFullGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { headGridPosition };

        foreach (SnakeMovePosition snakeMovePosition in movePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }

        return gridPositionList;
    }

    public class SnakeBodyPart
    {
        public SnakeMovePosition SnakeMovePosition;
        public Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeTailSprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.SnakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            int y = 0;

            //поворот тела змеи
            if (snakeMovePosition.GetPreviousSnakeMovePosition() != null)
            {
                switch (snakeMovePosition.GetPreviousSnakeMovePosition().GetDirection())
                {
                    default:
                    case InputManager.Direction.Up:
                        angle = 90;
                        break;
                    case InputManager.Direction.Down:
                        angle = -90;
                        y = 180;
                        break;
                    case InputManager.Direction.Left:
                        angle = 0;
                        y = 180;
                        break;
                    case InputManager.Direction.Right:
                        angle = 0;
                        break;
                }
            }
            else
            {
                // Если это первый элемент тела змеи, то устанавливаем направление на основе головы змеи
                switch (snakeMovePosition.GetDirection())
                {
                    default:
                    case InputManager.Direction.Up:
                        angle = 90;
                        break;
                    case InputManager.Direction.Down:
                        angle = -90;
                        y = 180;
                        break;
                    case InputManager.Direction.Left:
                        angle = 0;
                        y = 180;
                        break;
                    case InputManager.Direction.Right:
                        angle = 0;
                        break;
                }
            }

            transform.eulerAngles = new Vector3(0, y, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return SnakeMovePosition.GetGridPosition();
        }

    }

    public class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private InputManager.Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, InputManager.Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public InputManager.Direction GetDirection()
        {
            return direction;
        }

        public InputManager.Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return InputManager.Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }

        public SnakeMovePosition GetPreviousSnakeMovePosition()
        {
            return previousSnakeMovePosition;
        }
    }
}

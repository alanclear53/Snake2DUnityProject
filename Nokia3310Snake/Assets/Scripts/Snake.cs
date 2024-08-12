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
    private FoodContainer foodContainer;

    public int bodySize;

    private List<SnakeMovePosition> movePositionList;
    public List<SnakeBodyPart> bodyPartList;

    private Vector2Int startingPosition = new(15, 15);


    public void Setup(LevelGrid levelgrid, FoodContainer foodContainer)
    {
        this.levelgrid = levelgrid;
        this.foodContainer = foodContainer;

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

        if (this.IsHeadNearFood())
        {
            this.UpdateMouth(true);
        }
        else this.UpdateMouth(false);

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
        switch (headGridMoveDirection)
        {
            case InputManager.Direction.Right:
                headGridPosition += new Vector2Int(+1, 0);
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case InputManager.Direction.Left:
                headGridPosition += new Vector2Int(-1, 0);
                transform.eulerAngles = new Vector3(-180, 0, 180);
                break;
            case InputManager.Direction.Up:
                headGridPosition += new Vector2Int(0, +1);
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case InputManager.Direction.Down:
                headGridPosition += new Vector2Int(0, -1);
                transform.eulerAngles = new Vector3(0, 180, -90);
                break;
        }
    }

    private void UpdateMouth(bool isOpen)
    {
        Sprite sprite = isOpen ? GameAssets.instance.snakeHeadMouthOpenSprite : GameAssets.instance.snakeHeadSprite;
        this.transform.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public bool IsHeadNearFood()
    {
        foreach (Food food in foodContainer.GetFoodList())
        {
            if (Vector2.Distance(headGridPosition, food.GetPosition()) <= 1f)
            {
                return true;
            }
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
        for (int i = bodyPartList.Count - 1; i >= 0; i--)
        {
            SnakeMovePosition currentMovePosition = movePositionList[i];
            SnakeBodyPart bodyPart = bodyPartList[i];

            bodyPart.SetMovePosition(currentMovePosition);

            if (i == bodyPartList.Count - 1)
            {
                //хвост
                bodyPart.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeTailSprite;
            }
            else
            {
                SnakeMovePosition previousMovePosition = movePositionList[i + 1];

                if (currentMovePosition.GetDirection() != previousMovePosition.GetDirection())
                {
                    //угол
                    bodyPart.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodyCornerSprite;
                }
                else
                {
                    //обычный спрайт тела
                    bodyPart.transform.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
                    FoodDigestion(bodyPartList[i]);
                }
            }
        }
    }

    private void FoodDigestion(SnakeBodyPart bodyPart)
    {
        if (bodyPart == null) return;

        Vector2Int snakeBodyPartPositionInt = new Vector2Int(Mathf.RoundToInt(bodyPart.GetGridPosition().x), Mathf.RoundToInt(bodyPart.GetGridPosition().y));

        bodyPart.transform.GetComponent<SpriteRenderer>().sprite = levelgrid.cell[snakeBodyPartPositionInt.x, snakeBodyPartPositionInt.y].IsOccupiedByFood() && levelgrid.cell[snakeBodyPartPositionInt.x, snakeBodyPartPositionInt.y].IsOccupiedBySnake()
            ? GameAssets.instance.snakeBodyFullStomachSprite
            : GameAssets.instance.snakeBodySprite;
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
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y, 0);

            float angle = 0;
            float y = 0;

            switch (snakeMovePosition.GetDirection())
            {
                case InputManager.Direction.Up:
                    angle = 90;
                    if (snakeMovePosition.GetPreviousDirection() == InputManager.Direction.Left) angle = 0;
                    break;
                case InputManager.Direction.Down:
                    angle = -90;
                    y = 180;
                    if (snakeMovePosition.GetPreviousDirection() == InputManager.Direction.Left) y = 0;
                    break;
                case InputManager.Direction.Left:
                    angle = 0;
                    y = 180;
                    if (snakeMovePosition.GetPreviousDirection() == InputManager.Direction.Up) y = 0;
                    break;
                case InputManager.Direction.Right:
                    angle = 0;
                    if (snakeMovePosition.GetPreviousDirection() == InputManager.Direction.Up)
                    {
                        y = 180;
                    }
                    break;
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

        public SnakeMovePosition GetPreviousMovePosition()
        {
            return previousSnakeMovePosition;
        }
    }
}

using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Direction direction;
    public int moveCounter;

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    public Direction GetDirection()
    {
        bool leftPressed = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        bool rightPressed = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        if (moveCounter >= 1)
        {
            if (leftPressed && direction != Direction.Right)
            {
                direction = Direction.Left;
                moveCounter = 0;
            }
            if (rightPressed && direction != Direction.Left)
            {
                direction = Direction.Right;
                moveCounter = 0;
            }
            if (upPressed && direction != Direction.Down)
            {
                direction = Direction.Up;
                moveCounter = 0;
            }
            if (downPressed && direction != Direction.Up)
            {
                direction = Direction.Down;
                moveCounter = 0;
            }
        }


        return direction;
    }
}

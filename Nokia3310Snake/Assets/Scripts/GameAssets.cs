using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    public Sprite snakeHeadSprite;
    public Sprite snakeHeadMouthOpenSprite;
    public Sprite snakeBodySprite;
    public Sprite snakeBodyFullStomachSprite;
    public Sprite snakeTailSprite;
    public Sprite foodSprite;
    public Sprite bonusFoodSprite;
    public Sprite snakeBodyCornerSprite;

    private void Awake()
    {
        instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    public Sprite snakeHeadSprite;
    public Sprite snakeHeadMouthOpenSprite;
    public Sprite snakeBodySprite;
    public Sprite snakeBodyEatSprite;
    public Sprite snakeTailSprite;
    public Sprite foodSprite;

    private void Awake()
    {
        instance = this;
    }
}

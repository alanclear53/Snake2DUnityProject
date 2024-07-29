using UnityEngine;

public class Food : MonoBehaviour
{
    private Vector2Int position;
    private GameObject foodGameObject;

    public void Spawn(LevelGrid levelgrid)
    {
        position = new Vector2Int(Random.Range(1, levelgrid.width - 1), Random.Range(1, levelgrid.height - 1));

        while (levelgrid.cell[position.x, position.y].IsOccupiedBySnake())
        {
            position = new Vector2Int(Random.Range(1, levelgrid.width - 1), Random.Range(1, levelgrid.height - 1));
        }

        this.CreateGameObject();
        levelgrid.cell[position.x, position.y].SetOccupiedByFood();
    }

    private void CreateGameObject()
    {
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        Sprite foodSprite = GameAssets.instance.foodSprite;
        foodGameObject.GetComponent<SpriteRenderer>().sprite = foodSprite;
        foodGameObject.transform.position = new Vector3(position.x, position.y);
    }

    public void Destroy()
    {
        Object.DestroyImmediate(foodGameObject);
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
}

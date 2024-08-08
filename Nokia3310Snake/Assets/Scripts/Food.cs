using UnityEngine;

public abstract class Food : MonoBehaviour
{
    protected GameObject foodGameObject;
    protected FoodContainer container;

    public abstract void Spawn(LevelGrid levelGrid, FoodContainer container);
    public abstract int GetScore();

    public virtual void Destroy()
    {
        Object.DestroyImmediate(foodGameObject);
    }

    public abstract Vector2Int GetPosition();
    public abstract void RemoveFromContainer();
}

public class RegularFood : Food
{
    private Vector2Int position;

    public override int GetScore()
    {
        return 10;
    }

    public override void Spawn(LevelGrid levelGrid, FoodContainer container)
    {
        this.container = container;
        position = new Vector2Int(Random.Range(1, levelGrid.width - 1), Random.Range(1, levelGrid.height - 1));

        while (levelGrid.cell[position.x, position.y].IsOccupiedBySnake() || levelGrid.cell[position.x, position.y].IsOccupiedByFood())
        {
            position = new Vector2Int(Random.Range(1, levelGrid.width - 1), Random.Range(1, levelGrid.height - 1));
        }

        CreateGameObject();
        levelGrid.cell[position.x, position.y].SetOccupiedByFood();
        container.AddFood(this);
    }

    private void CreateGameObject()
    {
        foodGameObject = new GameObject("RegularFood", typeof(SpriteRenderer));
        Sprite foodSprite = GameAssets.instance.foodSprite;
        foodGameObject.GetComponent<SpriteRenderer>().sprite = foodSprite;
        foodGameObject.transform.position = new Vector3(position.x, position.y);
    }

    public override Vector2Int GetPosition()
    {
        return position;
    }

    public override void RemoveFromContainer()
    {
        container.RemoveFood(this);
    }
}

public class BonusFood : Food
{
    private Vector2Int position;

    public override int GetScore()
    {
        return 50;
    }

    public override void Spawn(LevelGrid levelGrid, FoodContainer container)
    {
        this.container = container; 
        position = new Vector2Int(Random.Range(1, levelGrid.width - 1), Random.Range(1, levelGrid.height - 1));

        while (levelGrid.cell[position.x, position.y].IsOccupiedBySnake() || levelGrid.cell[position.x, position.y].IsOccupiedByFood())
        {
            position = new Vector2Int(Random.Range(1, levelGrid.width - 1), Random.Range(1, levelGrid.height - 1));
        }

        CreateGameObject();
        levelGrid.cell[position.x, position.y].SetOccupiedByFood();
        container.AddFood(this); 
    }

    private void CreateGameObject()
    {
        foodGameObject = new GameObject("BonusFood", typeof(SpriteRenderer));
        Sprite foodSprite = GameAssets.instance.bonusFoodSprite;
        foodGameObject.GetComponent<SpriteRenderer>().sprite = foodSprite;
        foodGameObject.transform.position = new Vector3(position.x, position.y);
    }

    public override Vector2Int GetPosition()
    {
        return position;
    }

    public override void RemoveFromContainer()
    {
        container.RemoveFood(this);
    }
}
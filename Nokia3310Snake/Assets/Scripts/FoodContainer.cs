using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodContainer
{
    public Dictionary<Vector2Int, Food> foodDictionary;

    public FoodContainer()
    {
        foodDictionary = new Dictionary<Vector2Int, Food>();
    }

    public void AddFood(Food food)
    {
        foodDictionary.Add(food.GetPosition(), food);
    }

    public void RemoveFood(Food food)
    {
        foodDictionary.Remove(food.GetPosition());
        food.Destroy();
    }

    public Food GetFoodAtPosition(Vector2Int position)
    {
        return foodDictionary.TryGetValue(position, out Food food) ? food : null;
    }

    public void SpawnFood(LevelGrid levelGrid, int count, FoodType type)
    {
        for (int i = 0; i < count; i++)
        {
            Food food = FoodFactory.CreateFood(type);
            food.Spawn(levelGrid, this);
        }
    }

    public bool HasFood(FoodType type)
    {
        return foodDictionary.Any(f => f.GetType() == FoodFactory.GetFoodType(type));
    }

    public List<Food> GetFoodList()
    {
        return new List<Food>(foodDictionary.Values);
    }
}

    public enum FoodType
    {
        Regular,
        Bonus,
    }

    public static class FoodFactory
    {
        public static Food CreateFood(FoodType type)
        {
            switch (type)
            {
                case FoodType.Regular:
                    return new RegularFood();
                case FoodType.Bonus:
                    return new BonusFood();
                default:
                    throw new ArgumentException("Unknown food type", nameof(type));
            }
        }

        public static Type GetFoodType(FoodType type)
        {
            switch (type)
            {
                case FoodType.Regular:
                    return typeof(RegularFood);
                case FoodType.Bonus:
                    return typeof(BonusFood);
                default:
                    throw new ArgumentException("Unknown food type", nameof(type));
            }
        }
    }
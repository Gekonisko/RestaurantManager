using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pantry : MonoBehaviour {
    private static Dictionary<Meal, uint> mealsInPantry = new();

    public static void AddMealToPantry(Meal meal, uint amount) {
        if (!mealsInPantry.ContainsKey(meal))
            mealsInPantry.Add(meal, 0);
        mealsInPantry[meal] += amount;
    }

    public static void RemoveMealFromPantry(Meal meal, uint amount) {
        if (!mealsInPantry.ContainsKey(meal))
            throw new System.Exception("W spizarnia nie zawiera " + meal.ToString());
        if (mealsInPantry[meal] - amount < 0)
            throw new System.Exception("W spizarni znajduje sie " + mealsInPantry[meal].ToString() + " " + meal.ToString() + " a ty probijesz usunąć " + amount.ToString());
        mealsInPantry[meal] -= amount;
    }

    public static uint GetAmountOfMeal(Meal meal) {
        if (!mealsInPantry.ContainsKey(meal))
            return 0;
        return mealsInPantry[meal];
    }

    private void OnDestroy() {
        mealsInPantry.Clear();
    }
}
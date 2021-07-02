using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSetter : MonoBehaviour {

    [SerializeField] private GameObject g_meal;
    [SerializeField] private GameObject g_parentForMeals;

    public void SetMeals(List<MealData> meals) {
        foreach (MealData meal in meals) {
            Instantiate(g_meal, g_meal.transform.position, g_meal.transform.rotation, g_parentForMeals.transform).GetComponent<MealSetter>().SetMeal(meal);
        }
    }
}
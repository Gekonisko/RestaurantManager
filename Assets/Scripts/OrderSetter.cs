using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSetter : MonoBehaviour {

    [SerializeField] private GameObject g_meal;
    [SerializeField] private GameObject g_parentForMeals;
    private OrderData _orderData;

    public void SetParameters(OrderData orderData) {
        _orderData = orderData;
        SetMeals(_orderData.meals);
    }

    public void OnMouseDown() {
        if (IsEnoughMealsInPantry()) {
            GameEvents.SetComplitedOrder(_orderData.clientID);
            foreach (MealData meal in _orderData.meals)
                GameEvents.SetRemoveMealFromPantry(meal);
            Destroy(gameObject);
        }
    }

    private bool IsEnoughMealsInPantry() {
        foreach (MealData meal in _orderData.meals) {
            if (Pantry.GetAmountOfMeal(meal.meal) < meal.amount)
                return false;
        }
        return true;
    }

    private void SetMeals(List<MealData> meals) {
        foreach (MealData meal in meals) {
            Instantiate(g_meal, g_meal.transform.position, g_meal.transform.rotation, g_parentForMeals.transform).GetComponent<MealSetter>().SetMeal(meal);
        }
    }

}
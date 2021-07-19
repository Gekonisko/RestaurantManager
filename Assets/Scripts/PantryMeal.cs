using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PantryMeal : MonoBehaviour {

    [SerializeField] private Meal meal;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    private int _mealAmount;

    private System.IDisposable _cookedMealEvent, _removeMealEvent;

    private void Awake() {
        _cookedMealEvent = GameEvents.GetCookedMeal().Where(data => data.mealData.meal == meal).Subscribe(data => AddMealToPantry(data));
        _removeMealEvent = GameEvents.GetRemoveMealFromPantry().Where(data => data.meal == meal).Subscribe(data => RemoveMeal(data));
    }

    private void AddMealToPantry(CookedMealData data) {
        text.SetText((int.Parse(text.text) + data.mealData.amount).ToString());
        Pantry.AddMealToPantry(data.mealData.meal, data.mealData.amount);
    }

    private void RemoveMeal(MealData data) {
        _mealAmount = int.Parse(text.text);
        text.SetText((_mealAmount - data.amount).ToString());
        Pantry.RemoveMealFromPantry(data.meal, data.amount);
    }

    private void OnDestroy() {
        _cookedMealEvent?.Dispose();
        _removeMealEvent?.Dispose();
    }
}
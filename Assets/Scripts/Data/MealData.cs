using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct MealData {
    public Meal meal;
    public uint amount;

    public MealData(Meal meal, uint amount) {
        this.meal = meal;
        this.amount = amount;
    }
}
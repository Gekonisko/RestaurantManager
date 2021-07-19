using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CookedMealData {
    public uint cookID;
    public MealData mealData;
    public bool isSpoiled;

    public CookedMealData(uint cookID, MealData mealData, bool isSpoiled) {
        this.cookID = cookID;
        this.mealData = mealData;
        this.isSpoiled = isSpoiled;
    }
}
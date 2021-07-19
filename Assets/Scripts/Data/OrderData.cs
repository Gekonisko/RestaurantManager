using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct OrderData {
    public List<MealData> meals;
    public uint clientID;

    public OrderData(List<MealData> meals, uint clientID) {
        this.meals = meals;
        this.clientID = clientID;
    }
}
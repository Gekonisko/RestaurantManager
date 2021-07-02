using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPanelController : MonoBehaviour {

    // 0 = nie przydzielono kucharza
    [HideInInspector] public uint cookID = 0;

    public void SetCookingMeal(string meal) {
        if (cookID == 0)
            throw new System.Exception("Nie przydzielono kucharza - CookingPanel");

        foreach (string name in Enum.GetNames(typeof(Meal)))
            if (meal.Equals(name)) {
                GameEvents.SetCookTask(new CookTaskData(cookID, (Meal) Enum.Parse(typeof(Meal), meal)));
                cookID = 0;
                gameObject.SetActive(false);
                return;
            }
        throw new System.Exception("Nieprawidłowe wartości w przycikach CookingPanelu");

    }
}
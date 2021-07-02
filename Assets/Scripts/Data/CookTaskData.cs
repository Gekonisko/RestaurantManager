using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CookTaskData {
    public uint cookID;
    public Meal meal;

    public CookTaskData(uint cookID, Meal meal) {
        this.cookID = cookID;
        this.meal = meal;
    }
}
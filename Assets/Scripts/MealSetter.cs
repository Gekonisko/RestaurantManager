using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MealSetter : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    private string subfolder = "";

    public void SetMeal(MealData mealData) {
        _text.SetText(mealData.amount.ToString() + "x");
        try {
            _image.sprite = Resources.Load<ImageReference>(subfolder + mealData.meal.ToString()).image;
        } catch (System.NullReferenceException) {
            throw new System.Exception("Nie intnieje objekt o nazwie `" + mealData.meal.ToString() + "` w folderze Resources\\" + subfolder);
        }

    }
}
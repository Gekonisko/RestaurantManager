using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookUI : MonoBehaviour {
    public uint cookID = 0;

    public void ShowCookingPanel() {
        GameEvents.SetShowCookingPanel(cookID);
    }
}
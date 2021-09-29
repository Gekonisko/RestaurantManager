using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class TimerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _timeText;

    private IDisposable _dayTimeEvent;
    void Awake() {
        _dayTimeEvent = GameEvents.GetDayTime().Subscribe(data => UpdateTime(data));
    }

    private void UpdateTime(TimeData time) {
        _timeText.text = time.hours + ":" + ConvertMinutes(time.minutes);
    }

    private string ConvertMinutes(int minutes) {
        if (minutes < 10)
            return "0" + minutes;
        return minutes.ToString();
    }
}
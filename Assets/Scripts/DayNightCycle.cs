using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycle : MonoBehaviour {
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    [SerializeField] private bool isDayCycleOn = true;
    [SerializeField, Range(0, 24)] private float _timeOfDay;
    [SerializeField, Tooltip("Day Duration in Seconds")] private float _dayDuration = 24;
    private float _dayMultiplier = 1;
    private int _minutes, _actualMinutes = 0;
    private double _hours;

    private void Start() {
        _dayMultiplier = _dayDuration / 24;
    }

    private void Update() {
        if (!isDayCycleOn)
            return;
        if (Application.isPlaying) {
            _timeOfDay += Time.deltaTime / _dayMultiplier;
            _timeOfDay %= 24;
            UpdateTime(_timeOfDay);
            UpdateLighting(_timeOfDay / 24f);
        } else {
            UpdateLighting(_timeOfDay / 24f);
        }
    }

    private void UpdateTime(float time) {
        _hours = Math.Truncate(time);
        _minutes = (int) ((time - _hours) * 60);
        if (_minutes != _actualMinutes) {
            _actualMinutes = _minutes;
            GameEvents.SetDayTime(new TimeData((int) _hours, _minutes));
        }

    }

    private void UpdateLighting(float timePercent) {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
    }

    private void OnValidate() {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null) {
            DirectionalLight = RenderSettings.sun;
        } else {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights) {
                if (light.type == LightType.Directional) {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
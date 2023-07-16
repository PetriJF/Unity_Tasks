using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SliderHandler : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Slider slider;

    void Start() {
        valueText.text = slider.value.ToString("0.0");
    }

    void Update() {
        valueText.text = slider.value.ToString("0.0");
    }
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SliderHandler : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Slider slider;

    // Simply update the text representation fo the slider value at a 1 digit precision
    void Start() {
        valueText.text = slider.value.ToString("0.0");
    }

    // Simply update the text representation of the slider value at a 1 digit precision
    void Update() {
        valueText.text = slider.value.ToString("0.0");
    }
}


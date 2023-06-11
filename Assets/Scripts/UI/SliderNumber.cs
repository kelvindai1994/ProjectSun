using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

public class SliderNumber : MonoBehaviour
{
    [SerializeField] private Slider _sliderValue;
    [SerializeField] private TMP_Text _number;

    public void OnSliderValueChange()
    {
        _number.text = _sliderValue.value.ToString();
    }
}

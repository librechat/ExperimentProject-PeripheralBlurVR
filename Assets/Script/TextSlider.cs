using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour {

    [SerializeField]
    Text displayText;
    [SerializeField]
    Text descriptionText;
    [SerializeField]
    List<string> descriptionList;

    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChange);
    }

    void OnValueChange(float value)
    {
        displayText.text = (slider.wholeNumbers) ? value.ToString() : ((int)value).ToString();
        descriptionText.text = descriptionList[(int)value];
    }
}

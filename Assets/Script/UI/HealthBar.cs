using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Slider slider;

    public void SetMaxHelth(int helth)
    {
        slider.maxValue = helth;
        slider.value = helth;
    }
    public void SetHelth(int helth)
    {
        slider.value = helth;
    }
}

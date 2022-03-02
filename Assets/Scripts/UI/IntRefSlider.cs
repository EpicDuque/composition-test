using System;
using UnityEngine;
using UnityEngine.UI;

public class IntRefSlider : MonoBehaviour
{
    [SerializeField] private IntRef healthReference;
    [SerializeField] private IntRef maxHealthReference;

    private Slider slider;

    public IntRef HealthReference
    {
        get => healthReference;
        set => healthReference = value;
    }

    public IntRef MaxHealthReference
    {
        get => maxHealthReference;
        set => maxHealthReference = value;
    }

    private void Start()
    {
        slider = GetComponent<Slider>();

        // healthReference.ValueChanged.AddListener(OnHealthChanged);
        // MaxHealthReference.ValueChanged.AddListener(OnMaxHealthChanged);

        slider.value = healthReference.Value;
    }

    private void Update()
    {
        slider.value = healthReference.Value;
        slider.maxValue = maxHealthReference.Value;
        
    }
}

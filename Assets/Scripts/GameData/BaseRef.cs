using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRef<T> : ScriptableObject
{
    [SerializeField] private T value;
    [SerializeField, HideInInspector] protected UnityEvent<T> valueChanged;

    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            ValueChanged?.Invoke(value);
        }
    }

    public UnityEvent<T> ValueChanged => valueChanged;
}

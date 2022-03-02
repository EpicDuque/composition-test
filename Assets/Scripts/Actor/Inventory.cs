using CoolTools.Attributes;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField, InspectorDisabled] private int lootAmount;

    public int LootAmount
    {
        get => lootAmount;
        set => lootAmount = value;
    }
}

using UnityEngine;

public class InventoryInteractionCondition : InteractionCondition
{
    [SerializeField] private int requiredAmount = 0;
    
    public override bool Evaluate(Interactor interactor = null)
    {
        if (interactor == null) return false;

        if (!interactor.TryGetComponent<Inventory>(out var inventory)) return false;

        return inventory.LootAmount >= requiredAmount;
    }
}

using UnityEngine;

public class HealthInteractionCondition : InteractionCondition
{
    [SerializeField] private int minHealth;
    
    public override bool Evaluate(Interactor interactor = null)
    {
        if (interactor == null) return false;

        if (!interactor.TryGetComponent<Damageable>(out var damageable)) return false;

        return damageable.Health > minHealth;
    }
}

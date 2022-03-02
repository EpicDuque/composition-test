
using UnityEngine;

public class DestroyInteraction : InteractionType
{
    [SerializeField] private float delay = 0.15f;
    
    public override void Execute(Interactor interactor = null)
    {
        Destroy(gameObject, delay);
    }
}

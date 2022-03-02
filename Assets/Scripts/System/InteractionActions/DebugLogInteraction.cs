using UnityEngine;
using UnityEngine.Events;

public class DebugLogInteraction : InteractionType
{
    [SerializeField] private string message;
    
    public override void Execute(Interactor interactor = null)
    {
        Debug.Log($"DebugLogInteraction: {message}", this);

        if (interactor == null) return;
        
        Debug.Log($"Interactor: {interactor.name}", interactor);
    }
}

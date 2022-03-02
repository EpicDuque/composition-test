using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform castPoint;
    [SerializeField] private float radius = 4f;

    private Interactable currentInteractabale;

    public Interactable CurrentInteractabale => currentInteractabale;
    

    // From player input
    private void OnInteract(InputValue value)
    {
        var interactable = CastInteractionBox();

        DoInteraction(interactable);
    }
    
    public Interactable CastInteractionBox()
    {
        var colliders = new Collider[20];
        
        var size = Physics.OverlapSphereNonAlloc(castPoint.position, 4f, colliders);

        if(size <= 0) return null;
        
        var col = colliders
            .FirstOrDefault(c => c.TryGetComponent<Interactable>(out _));

        if (col == null) return null;
        
        var interactable = col.GetComponent<Interactable>();

        return interactable;
    }

    public void DoInteraction(Interactable interactable)
    {
        if (interactable == null) return;
        
        interactable.DoInteractions(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(castPoint.position, radius);
    }
}

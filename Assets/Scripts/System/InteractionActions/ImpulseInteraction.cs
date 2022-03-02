using UnityEngine;

public class ImpulseInteraction : InteractionType
{
    [SerializeField] private float force = 10f;
    [SerializeField] private Rigidbody body;
    
    public override void Execute(Interactor interactor = null)
    {
        if (body == null) return;
        if(interactor == null) return;
        
        var impulse = (transform.position - interactor.transform.position).normalized * force;
        
        body.AddForce(impulse, ForceMode.Impulse);
    }
}

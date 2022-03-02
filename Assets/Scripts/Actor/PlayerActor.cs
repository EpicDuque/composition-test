using CoolTools.Actors;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActor : Actor
{
    private void OnMove(InputValue value)
    {
        Movement = value.Get<Vector2>();
        
        if (Movement != Vector2.zero)
            LookTowardsMovement();
    }
}

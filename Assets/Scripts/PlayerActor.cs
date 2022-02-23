using CoolTools.Actors;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActor : Actor
{
    private void OnMove(InputValue value)
    {
        Movement = value.Get<Vector2>();
    }

    private new void Update()
    {
        base.Update();
        
        if(Movement != Vector2.zero)
            LookDirection = new Vector3(Movement.x, 0, Movement.y);
    }
}

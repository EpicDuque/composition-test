using CoolTools.Actors;
using UnityEngine;

public class EnemyActor : Actor
{
    // Nothing to do here, this just represents an enemy

    private new void Update()
    {
        base.Update();
        
        if(Movement != Vector2.zero)
            LookDirection = new Vector3(Movement.x, 0, Movement.y);
    }
}

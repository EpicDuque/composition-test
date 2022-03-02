using UnityEngine;

public abstract class InteractionCondition : MonoBehaviour
{
    public abstract bool Evaluate(Interactor interactor = null);
}

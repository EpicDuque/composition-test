using UnityEngine;

public abstract class InteractionType : MonoBehaviour
{
    public abstract void Execute(Interactor interactor = null);
}
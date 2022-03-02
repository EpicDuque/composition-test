using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private List<InteractionType> actions;
    [SerializeField] private List<InteractionCondition> conditions;
    
    public void DoInteractions(Interactor interactor = null)
    {
        if (!EvaluateConditions(interactor)) return;

        actions.ForEach(action => action.Execute(interactor));
    }
    
    public bool EvaluateConditions(Interactor interactor = null) => 
        conditions.All(cond => cond.Evaluate(interactor));
}

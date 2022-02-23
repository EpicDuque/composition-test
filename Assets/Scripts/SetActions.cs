using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetActions : MonoBehaviour
{

    [Serializable]
    public struct SetAction
    {
        public float Delay;
        public UnityEvent Action;
    }

    [SerializeField] private bool restartable;
    
    [Space(10f)]
    [SerializeField] private SetAction[] actions;

    [Space(10f)] 
    [SerializeField] private UnityEvent actionsStarted;
    [SerializeField] private UnityEvent<SetAction> actionPerformed;
    [SerializeField] private UnityEvent actionsFinished;

    public UnityEvent ActionsStarted => actionsStarted;
    public UnityEvent ActionsFinished => actionsFinished;
    public UnityEvent<SetAction> ActionPerformed => actionPerformed;
    
    private Coroutine routine;

    public void DoActions()
    {
        if (routine != null && !restartable) return;
        
        if(routine != null)
            StopCoroutine(routine);
        
        routine = StartCoroutine(ActionSequenceRoutine());
    }

    private IEnumerator ActionSequenceRoutine()
    {
        actionsStarted?.Invoke();
        
        foreach (var ac in actions)
        {
            yield return new WaitForSeconds(ac.Delay);
            ac.Action?.Invoke();
            actionPerformed?.Invoke(ac);
        }

        actionsFinished?.Invoke();
        routine = null;
    }
}

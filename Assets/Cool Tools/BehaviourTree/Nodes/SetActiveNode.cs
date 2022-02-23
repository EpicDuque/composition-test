using CoolTools.BehaviourTree;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class SetActiveNode : ActionNode
    {
        private GameObject target;

        [SerializeField] private string targetVariable;
        [SerializeField] private NodeProperty<bool> toggle;
        
        protected override void OnStart()
        {
    #if VISUAL_SCRIPTING
            target = Tree.Variables.declarations.Get<GameObject>(targetVariable);
    #else
            target = Tree.Blackboard.GetRefValue<GameObject>(targetVariable);
    #endif

        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (target == null)
            {
                Debug.LogError("Null GameObject reference...", this);
                return State.Failure;
            }
            
            target.SetActive(toggle.Value);
            return State.Success;
        }
    }
}

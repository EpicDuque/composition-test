using Experimental.BehaviourTree;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class IsActiveNode : InterruptNode
    {
        [SerializeField] private string targetVariableName;

        private GameObject target;
        
        protected override void OnStart()
        {
#if VISUAL_SCRIPTING
            target = Tree.Variables.declarations.Get<GameObject>(targetVariableName);
#else
            target = Tree.Blackboard.GetRefValue<GameObject>(targetVariableName);
#endif
        }

        protected override State OnUpdate()
        {
            condition.Value = target.activeSelf;
            return base.OnUpdate();
        }
    }
}

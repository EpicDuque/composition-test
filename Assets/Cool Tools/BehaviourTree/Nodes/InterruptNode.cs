using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class InterruptNode : DecoratorNode
    {
        [SerializeField] protected NodeProperty<bool> condition;
        
        protected override void OnStart()
        {
            state = State.Running;
        }

        protected override void OnStop()
        {
            Tree.Traverse(Child, node =>
            {
                node.Stop();
                node.started = false;
                node.state = State.Idle;
            });
        }

        protected override State OnUpdate()
        {
            return condition.Value ?
                Child != null ? Child.Update() : State.Success : State.Failure;
        }
    }
}

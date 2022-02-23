using CoolTools.BehaviourTree;

namespace Experimental.BehaviourTree
{
    public class FailNode : ActionNode
    {
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            return State.Failure;
        }
    }
}
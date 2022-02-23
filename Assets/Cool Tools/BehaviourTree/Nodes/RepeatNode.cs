namespace CoolTools.BehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (Child == null) return State.Failure;
            Child.Update();

            return State.Running;

        }
    }
}
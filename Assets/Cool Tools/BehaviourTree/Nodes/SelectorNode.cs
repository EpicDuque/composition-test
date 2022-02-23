namespace CoolTools.BehaviourTree
{
    public class SelectorNode : SequencerNode
    { 
        protected override bool CanEvaluateNextChild(State s) => s == State.Failure;
    }
}

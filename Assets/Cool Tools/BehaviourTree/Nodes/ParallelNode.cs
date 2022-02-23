namespace CoolTools.BehaviourTree
{
    public class ParallelNode : SequencerNode
    {
        // Can Always evaluate next child on same tick
        protected override bool CanEvaluateNextChild(State s) => true;

        protected override State EvaluationLoopState()
        {
            return State.Success;
        }
    }
}

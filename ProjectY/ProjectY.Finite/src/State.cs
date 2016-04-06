using System.Collections.Generic;

namespace ProjectY.Finite
{
    public class State : IState
    {
        #region Data

        private List<Character> inputs;
        private List<IState> transitions;

        public StateMachine Parent { get; private set; }
        #endregion

        #region Constructors

        public State(StateMachine parent)
        {
            inputs = new List<Character>();
            transitions = new List<IState>();
            this.Parent = parent;
        }

        #endregion

        #region Interface

        public void AddTransition(IState transition, Character input)
        {
            inputs.Add(input);
            transitions.Add(transition);
        }

        public HashSet<StatePath> OnInput(char input)
        {
            HashSet<StatePath> next = new HashSet<StatePath>();
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] != input) continue;

                next.Add(new StatePath(transitions[i]));
            }
            return next;
        }

        public HashSet<StatePath> OnInput(char[] input)
        {
            HashSet<StatePath> next = new HashSet<StatePath>();
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] != input) continue;

                var nextPath = new StatePath(transitions[i]);

                if (inputs[i] == StateMachine.EPS_EXIT)
                {
                    nextPath.Marked = true;
                }

                next.Add(nextPath);
            }
            return next;
        }

        #endregion

    }
}
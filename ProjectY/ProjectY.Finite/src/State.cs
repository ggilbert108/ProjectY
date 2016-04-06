using System;
using System.Collections.Generic;

namespace ProjectY.Finite
{
    public class State : IState
    {
        #region Data

        private List<Character> transitions;
        private List<IState> states;

        public StateMachine Parent { get; private set; }

        private static int currentId = 0;
        private int id;
        #endregion

        #region Constructors

        public State(StateMachine parent)
        {
            transitions = new List<Character>();
            states = new List<IState>();
            this.Parent = parent;

            id = currentId++;
        }

        #endregion

        #region Interface

        public void AddTransition(IState transition, Character input)
        {
            transitions.Add(input);
            states.Add(transition);
        }

        public HashSet<StatePath> OnInput(char input)
        {
            HashSet<StatePath> next = new HashSet<StatePath>();
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i] != input) continue;

                next.Add(new StatePath(states[i]));
            }
            return next;
        }

        public HashSet<StatePath> OnInput(char[] input)
        {
            HashSet<StatePath> next = new HashSet<StatePath>();
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i] != input) continue;

                var nextPath = new StatePath(states[i]);

                if (transitions[i] == StateMachine.EPS_EXIT)
                {
                    nextPath.Marked = true;
                }

                next.Add(nextPath);
            }
            return next;
        }

        #endregion

        public void Print()
        {
            Console.WriteLine(id);
            for(int i = 0; i < transitions.Count; i++)
            {
                string ch;
                if (transitions[i] == StateMachine.EPS_EXIT) ch = "EXIT";
                else if (transitions[i] == StateMachine.EPS_STANDARD) ch = "EPS";
                else ch = transitions[i].ToString();
                Console.WriteLine($"\t{ch} -> ");
            }

            foreach (IState state in states)
            {
                state.Print();
            }
        }
    }
}
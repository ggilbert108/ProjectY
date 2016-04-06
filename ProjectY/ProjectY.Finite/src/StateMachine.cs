using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectY.Finite
{
    public class StateMachine : IState
    {
        #region Data

        #region Const And Static

        //input is guaranteed to be ASCII

        public readonly char[] Eps = {EPS_STANDARD, EPS_EXIT};
        public const char EPS_STANDARD = (char)(byte.MaxValue + 1);
        public const char EPS_EXIT = (char) (byte.MaxValue + 2);

        #endregion

        private readonly State initial;
        private readonly State final;
        private HashSet<Character> inputs;
        private bool untouched;

        public string Captured { get; private set; }

        #endregion

        #region Constructors

        public StateMachine()
        {
            initial = new State(this);
            final = new State(this);
            inputs = new HashSet<Character>();

            untouched = true;
            Captured = "";
        }

        #endregion

        #region Simulate

        public bool Validate(string code)
        {
            HashSet<StatePath> currentStates = new HashSet<StatePath> { new StatePath(initial) };
            currentStates.UnionWith(EpsClosure(currentStates));

            foreach (char input in code)
            {
                if (!Util.CharacterCollectionContainsChar(inputs, input))
                    return false;

                currentStates = Next(currentStates, input);
                currentStates.UnionWith(EpsClosure(currentStates));
            }

            var states =
                from path in currentStates
                select path.State;

            return states.Contains(final);
        }

        #region Helpers

        HashSet<StatePath> Next(HashSet<StatePath> prevStates, char input)
        {
            HashSet<StatePath> next = new HashSet<StatePath>();

            foreach (StatePath prevState in prevStates)
            {
                var fromPrev = prevState.State.OnInput(input);

                foreach(StatePath from in fromPrev)
                {
                    from.Recorded = prevState.Recorded + input;
                }

                next.UnionWith(fromPrev);
            }

            return next;
        }

        HashSet<StatePath> EpsClosure(HashSet<StatePath> prevStates)
        {
            HashSet<StatePath> closure = new HashSet<StatePath>();
            Stack<StatePath> dfsStack = new Stack<StatePath>();

            foreach (StatePath prevState in prevStates)
            {
                closure.Add(prevState);
                dfsStack.Push(prevState);
            }

            while (dfsStack.Count > 0)
            {
                StatePath state = dfsStack.Pop();
                HashSet<StatePath> adjacents = state.State.OnInput(Eps);

                foreach (StatePath adjacent in adjacents)
                {
                    adjacent.Recorded += state.Recorded;
                }

                var exits =
                    from adjacent in adjacents
                    where adjacent.Marked
                    select adjacent;

                foreach (var exit in exits)
                {
                    State machineFinalState = (State)exit.State;

                    machineFinalState.Parent.Captured = exit.Recorded;
                }

                foreach (StatePath adjacent in adjacents)
                {
                    if (!closure.Contains(adjacent))
                    {
                        closure.Add(adjacent);
                        dfsStack.Push(adjacent);
                    }
                }
            }

            return closure;
        }

        #endregion

        #endregion

        #region Interface

        public void AddTransition(IState transition, Character input)
        {
            final.AddTransition(transition, input);
        }

        public HashSet<StatePath> OnInput(char input)
        {
            //when execution decends into a StateMachine, reset 'recorded'
            var result = initial.OnInput(input);

            foreach (StatePath path in result)
            {
                path.Recorded = "" + input;
            }

            return result;
        }

        public HashSet<StatePath> OnInput(char[] input)
        {
            var result = initial.OnInput(input);

            foreach (StatePath path in result)
            {
                path.Recorded = "" + input;
            }

            return result;
        }

        #endregion

        #region Util

        private void AddInputsFromOther(StateMachine other)
        {
            inputs.UnionWith(other.inputs);
        }

        #endregion

        #region Construction

        public void Encapsulate(StateMachine inner)
        {
            if (!untouched)
                throw new Exception("You can only encapsulate if the FSM hasn't been altered from it's original state");

            AddInputsFromOther(inner);

            initial.AddTransition(inner, EPS_STANDARD);
            inner.AddTransition(final, EPS_EXIT);
        }

        #region Builders

        public static StateMachine BuildBasic(Character character)
        {
            StateMachine basic = new StateMachine();

            basic.inputs.Add(character);

            basic.initial.AddTransition(basic.final, character);

            return basic;
        }

        public static StateMachine BuildAlternation(params StateMachine[] machines)
        {
            StateMachine alt = new StateMachine();

            foreach (StateMachine machine in machines)
            {
                alt.AddInputsFromOther(machine);
                machine.untouched = false;

                alt.initial.AddTransition(machine.initial, EPS_STANDARD);
                machine.final.AddTransition(alt.final, EPS_STANDARD);
            }

            return alt;
        }

        public static StateMachine BuildConcatenation(params StateMachine[] machines)
        {
            StateMachine concat = null;
            foreach (StateMachine machine in machines)
            {
                machine.untouched = false;
                concat = concat == null ? machine : BuildConcatenation(concat, machine);
            }

            return concat;
        }

        private static StateMachine BuildConcatenation(StateMachine a, StateMachine b)
        {
            StateMachine concat = new StateMachine();

            concat.AddInputsFromOther(a);
            concat.AddInputsFromOther(b);

            concat.initial.AddTransition(a.initial, EPS_STANDARD);
            a.final.AddTransition(b.initial, EPS_STANDARD);
            b.final.AddTransition(concat.final, EPS_STANDARD);

            return concat;
        }

        #endregion

        #endregion

        public void Print()
        {
            initial.Print();
        }
    }
}
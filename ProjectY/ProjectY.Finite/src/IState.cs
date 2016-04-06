using System.Collections.Generic;

namespace ProjectY.Finite
{
    public interface IState
    {   
        void AddTransition(IState transition, Character input);
        HashSet<StatePath> OnInput(char input);
        HashSet<StatePath> OnInput(char[] input);
    }
}
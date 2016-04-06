using System;
using ProjectY.Finite;

namespace ProjectY.Test
{
    public class Program 
    {
        public static void Main()
        {
            StateMachine a = StateMachine.BuildBasic('a');
            StateMachine b = StateMachine.BuildBasic('b');
            StateMachine aWrapper = new StateMachine();
            StateMachine bWrapper = new StateMachine();

            aWrapper.Encapsulate(a);
            bWrapper.Encapsulate(b);


            StateMachine alt = StateMachine.BuildAlternation(aWrapper, bWrapper);

            alt.Print();

            alt.Validate("a");

            Console.WriteLine(aWrapper.Captured);
        }
    }
}
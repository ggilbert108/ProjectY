using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectY.Finite;

namespace ProjectY.Test
{
    [TestFixture]
    public class NfaTest
    {
        [TestCase("a", "a", false, ExpectedResult = true)]
        [TestCase("a", "b", false, ExpectedResult = false)]
        [TestCase("m", "a-z", true, ExpectedResult = true)]
        [TestCase("M", "a-z", true, ExpectedResult = false)]
        public bool TestBasic(string input, string ch, bool isPattern)
        {
            Character character = isPattern ? new Character(ch) : new Character(ch[0]);
            StateMachine nfa = StateMachine.BuildBasic(character);

            return nfa.Validate(input);
        }

        [TestCase("a", "a", "b", ExpectedResult = true)]
        [TestCase("b", "a", "b", ExpectedResult = true)]
        public bool TestAlternation(string input, params string[] against)
        {
            var machines = new List<StateMachine>();
            foreach (var str in against)
            {
                Character character;
                if (str.Length == 1)
                    character = str[0];
                else
                    character = str;

                machines.Add(StateMachine.BuildBasic(character));
            }

            StateMachine alt = StateMachine.BuildAlternation(machines.ToArray());

            return alt.Validate(input);
        }

        [Test]
        public void TestCapture()
        {
            StateMachine a = StateMachine.BuildBasic('a');
            StateMachine b = StateMachine.BuildBasic('b');
            StateMachine aWrapper = new StateMachine();
            StateMachine bWrapper = new StateMachine();

            aWrapper.Encapsulate(a);
            bWrapper.Encapsulate(b);
            

            StateMachine alt = StateMachine.BuildAlternation(aWrapper, bWrapper);

            alt.Validate("a");

            Assert.AreEqual("a", aWrapper.Captured);
        }
    }
}
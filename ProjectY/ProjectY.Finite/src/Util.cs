using System.Collections.Generic;

namespace ProjectY.Finite
{
    public class Util
    {
        public static bool CharacterCollectionContainsChar(
           IEnumerable<Character> collection,
           char ch)
        {
            foreach (Character character in collection)
            {
                if (character == ch)
                    return true;
            }
            return false;
        }
    }
}
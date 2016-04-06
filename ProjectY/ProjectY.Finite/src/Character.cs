using System;
using System.Text.RegularExpressions;

namespace ProjectY.Finite
{
    public class Character
    {
        #region Data

        private readonly string value;
        private readonly bool isPattern;
        private string exceptions;

        #endregion

        #region Constructors

        public Character(string pattern, string exceptions = "")
        {
            if (pattern[0] == '[')
            {
                throw new ArgumentException("Do not put brackets around the character classes");
            }
            value = pattern;
            isPattern = true;
            this.exceptions = exceptions;
        }

        public Character(char value)
        {
            this.value = value + "";
            isPattern = false;
        }

        #endregion

        #region Cast Operators

        public static implicit operator Character(char value)
        {
            return new Character(value);
        }

        public static implicit operator Character(string pattern)
        {
            return new Character(pattern);
        }

        #endregion

        #region Other Operators (-)

        public static Character operator -(Character character, char ch)
        {
            return new Character(character.value, character.exceptions + ch);
        }

        #endregion

        #region Equality

        #region Equality Operators

        public static bool operator ==(Character character, char[] chs)
        {
            foreach (char ch in chs)
            {
                if (character == ch)
                    return true;
            }
            return false;
        }

        public static bool operator !=(Character character, char[] chs)
        {
            return !(character == chs);
        }

        public static bool operator ==(Character character, char ch)
        {
            if (character.isPattern)
            {
                return Regex.IsMatch("" + ch, character.Pattern);
            }
            else
            {
                return character.value[0] == ch;
            }
        }

        public static bool operator !=(Character character, char ch)
        {
            return !(character == ch);
        }

        #endregion

        #region Methods that have to be implemented so ReSharper doesn't yell

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            int code = value.GetHashCode();
            if (isPattern)
            {
                code *= 2;
            }
            return code;
        }

        #endregion

        #endregion

        #region Properties

        private string Pattern
        {
            get
            {
                if (!isPattern)
                    throw new Exception("Can't access pattern on plain character");

                if (exceptions.Length == 0)
                    return $"[{value}]";
                else
                    return $"[{value}-[{exceptions}]]";
            }
        }

        #endregion
    }
}
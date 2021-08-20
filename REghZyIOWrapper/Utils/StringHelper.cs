using System.Collections.Generic;
using System.Text;

namespace REghZyIOWrapper.Utils {
    /// <summary>
    /// A class i made that provides a bunch of helpful functions for "manipulating" strings
    /// </summary>
    public static class StringHelper {
        /// <summary>
        /// Java's substring, where you supply a start index (inclusive) and end index (exclusive)
        /// </summary>
        /// <param name="value">The string value to substring</param>
        /// <param name="startIndex">The start index (inclusive, aka extract the char at this index)</param>
        /// <param name="endIndex">The end index (exclusive, aka do not extract the char at this index, but extract the char before it)</param>
        /// <returns>The substring'd string</returns>
        public static string JSubstring(this string value, int startIndex, int endIndex) {
            return value.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Java's substring, where you supply a start index (inclusive)
        /// </summary>
        /// <param name="value">The string value to substring</param>
        /// <param name="startIndex">The start index (inclusive, aka extract the char at this index)</param>
        /// <returns>The substring'd string</returns>
        public static string JSubstring(this string value, int startIndex) {
            return value.Substring(startIndex);
        }

        /// <summary>
        /// Extracts and includes the characters between <paramref name="startIndex"/> and <paramref name="endIndex"/>
        /// </summary>
        /// <param name="value">The string value to substring</param>
        /// <param name="startIndex">The start index (inclusive, aka extract the char at this index)</param>
        /// <param name="endIndex">The end index (inclusive, aka extract the char at this index)</param>
        /// <returns>The substring'd string</returns>
        public static string Extract(this string value, int startIndex, int endIndex) {
            return value.Substring(startIndex, endIndex + 1 - startIndex);
        }

        /// <summary>
        /// Extracts the results between the first occourance of <paramref name="a"/> and the first 
        /// occourance of <paramref name="b"/> (<paramref name="b"/>'s search starts at the index of a).
        /// If either <paramref name="a"/> or <paramref name="b"/> could't be found, null is returned.
        /// Otherwise, the value between, or empty, is returned
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <param name="a">The string to search</param>
        /// <param name="b"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string Between(this string value, string a, string b, int startIndex = 0) {
            int indexA = value.IndexOf(a, startIndex);
            if (indexA == -1)
                return null;

            int indexB = value.IndexOf(b, indexA);
            if (indexB == -1)
                return null;

            return value.Extract(indexA + a.Length, indexB - 1);
        }

        /// <summary>
        /// Gets the value in <paramref name="text"/> before <paramref name="beforeThis"/>. 
        /// If <paramref name="beforeThis"/> couldn't be found in <paramref name="text"/>, null is returned.
        /// </summary>
        /// <param name="text">The text to search</param>
        /// <param name="beforeThis">The text to look for</param>
        /// <param name="startIndex">The index to start searching at in <paramref name="text"/> (inclusive)</param>
        /// <returns></returns>
        public static string Before(this string text, string beforeThis, int startIndex = 0) {
            int index = text.IndexOf(beforeThis, startIndex);
            if (index == -1)
                return null;

            return text.Substring(0, index);
        }


        public static string After(this string value, string afterThis, int startIndex = 0) {
            int index = value.IndexOf(afterThis, startIndex);
            if (index == -1) {
                return null;
            }

            return value.Substring(index + afterThis.Length);
        }

        /// <summary>
        /// Returns true if the text is null or empty
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string text) {
            return text == string.Empty;
        }

        /// <summary>
        /// Repeats the given string value, a given number of times. 
        /// 0 = no text, 1 = the value, 2 = the value (twice), etc
        /// </summary>
        /// <param name="value">The value to be repeated</param>
        /// <param name="n">The number of times to repeat the value</param>
        /// <returns>A new string with the repeated text</returns>
        public static string Repeat(this string value, int n) {
            StringBuilder sb = new StringBuilder(n * value.Length);
            for (int i = 0; i < n; i++) {
                sb.Append(value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Repeats the given value, a given number of times.
        /// 0 = no text, 1 = the value, 2 = the value (twice), etc
        /// </summary>
        /// <param name="value">The character to be repeated</param>
        /// <param name="n">The number of times to repeat the value</param>
        /// <returns>A new string with the repeated text</returns>
        public static string Repeat(this char value, int n) {
            StringBuilder sb = new StringBuilder(n);
            for (int i = 0; i < n; i++) {
                sb.Append(value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Forces a given string to be a certain length by filling it with an excess of a given character (whitespace by default),
        /// or by removing characters on the end if you require a shorter string
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="length">The length of the string that will be returned</param>
        /// <param name="fillCharacter">The character that is added if the given text is too short</param>
        /// <returns></returns>
        public static string EnsureLength(this string text, int length, char fillCharacter = ' ') {
            int repeatLength = length - text.Length;
            if (repeatLength == 0)
                return text;
            else if (repeatLength < 0)
                return text.Remove(length);
            else {
                return text + Repeat(fillCharacter, repeatLength);
            }
        }

        /// <summary>
        /// Counts the total number of occourances of a given character
        /// </summary>
        /// <param name="value"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int Count(this string value, char character) {
            int counts = 0;
            for (int i = 0; i < value.Length; i++) {
                if (value[i] == character)
                    counts++;
            }
            return counts;
        }

        /// <summary>
        /// Replaces all occourances of repeated whitespaces with single whitespaces
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A string where all occourances of whitespaces are always 1 character long</returns>
        public static string CollapseWhitespaces(this string value) {
            //return Regex.Replace(value, @"\s+", " ");
            return value.CollapseCharacter(' ');
        }

        /// <summary>
        /// Replaces all repeated occourances of the given character with just 1 occourance
        /// </summary>
        /// <param name="value"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string CollapseCharacter(this string value, char character) {
            string[] values = value.Split(character);
            List<string> newValues = new List<string>(values.Length);
            for (int i = 0; i < values.Length; i++) {
                string val = values[i];
                if (val.Trim() == string.Empty)
                    continue;
                newValues.Add(val);
            }
            return string.Join(character.ToString(), newValues);
        }

        /// <summary>
        /// (probably) Efficienctly make every character of value lower case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EfficientToLower(this string value) {
            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++) {
                char character = value[i];
                if (character.IsUppercase())
                    sb.Append(character.ForceLowercase());
                else
                    sb.Append(character);
            }

            return sb.ToString();
        }

        /// <summary>
        /// (probably) Efficienctly make every character of value upper case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EfficientToUpper(this string value) {
            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++) {
                char character = value[i];
                if (character.IsLowercase())
                    sb.Append(character.ForceUppercase());
                else
                    sb.Append(character);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns whether the character's integer value falls between the lower case region of unicode characters (97-122)
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsLowercase(this char character) {
            return character > 96 && character < 123;
        }

        /// <summary>
        /// Returns whether the character's integer value falls between the upper case region of unicode characters (65-90)
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsUppercase(this char character) {
            return character > 64 && character < 91;
        }

        /// <summary>
        /// Subtracts 32 from the given character value. characters should
        /// be lowercase otherwise it will return weird stuff (this function doesn't check, hense "ForceUppercase")
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ForceUppercase(this char value) {
            return (char)(value - 32);
        }

        /// <summary>
        /// Adds 32 from the given character value. characters should
        /// be uppercase otherwise it will return weird stuff 
        /// (this function doesn't check, hense "ForceLowercase")
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ForceLowercase(this char value) {
            return (char)(value + 32);
        }
    }
}

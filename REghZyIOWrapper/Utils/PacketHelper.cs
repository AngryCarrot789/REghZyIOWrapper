using System;
using System.Xml.Schema;
using REghZyIOWrapper.Exceptions;

namespace REghZyIOWrapper.Utils {
    public static class PacketHelper {
        public static int ParseInt(this string value) {
            if (int.TryParse(value, out int i)) {
                return i;
            }

            throw new PacketCreationException($"Failed to parse the value '{value}' to an integer");
        }

        public static double ParseDouble(this string value) {
            if (double.TryParse(value, out double d)) {
                return d;
            }

            throw new PacketCreationException($"Failed to parse the value '{value}' to a double");
        }

        public static T ParseEnum<T>(this int value) where T : struct {
            if (Enum.TryParse<T>(value.ToString(), out T result)) {
                return result;
            }

            throw new PacketCreationException($"Failed to parse the int value '{value}' to the enum type '{typeof(T).Name}'");
        }

        public static T ParseEnum<T>(this string value) where T : struct {
            if (Enum.TryParse<T>(value, out T result)) {
                return result;
            }

            throw new PacketCreationException($"Failed to parse the string value '{value}' to the enum type '{typeof(T).Name}'");
        }

        /// <summary>
        /// Checks if the given value is between (or equal to, if inclusive is true) the given min and max value
        /// <code>
        /// IsBetween(10, 10, 12, true) == true
        /// </code>
        /// <code>
        /// IsBetween(10, 10, 12, false) == false
        /// </code>
        /// <code>
        /// IsBetween(11, 10, 12, false) == true
        /// </code>
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="inclusive">
        /// If true, the value will be checked if it's between min and max, or equal to either of them.
        /// If false, the value will only be checked if it's between
        /// </param>
        /// <returns></returns>
        public static bool IsBetween(this int value, int min, int max, bool inclusive = true) {
            if (inclusive) {
                return value >= min && value <= max;
            }
            else {
                return value > min && value < max;
            }
        }

        /// <summary>
        /// Returns a string between the index of the given character, and the next index of the given character, where the word-index is of the given index
        /// <para>
        /// SubstringWordBetween("hello.there.xd.lol", 0 '.') == "hello"
        /// </para>
        /// <para>
        /// SubstringWordBetween("hello.there.xd.lol", 2 '.') == "xd"
        /// </para>
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <param name="splitter">The character that splits the words (new line, whitespace, etc)</param>
        /// <param name="index">The word index</param>
        /// <returns></returns>
        public static string GetWordAt(this string value, int index, char splitter = '.') {
            int indexA = -1;
            int indexB = value.IndexOf(splitter);
            if (indexB == -1) {
                throw new IndexOutOfRangeException($"The value ({value}) did not contain the given character '{splitter}' (tried to index at {index})");
            }

            for(int i = 0; i < index; i++) {
                if (indexB == -1 || indexB >= value.Length) {
                    throw new IndexOutOfRangeException($"The value ({value}) didn't contain a '{splitter}' character past the index '{indexA}' (tried to index at {index}, but the value only contained {value.Count(splitter)} of the character)");
                }

                indexA = indexB;
                indexB = value.IndexOf(splitter, indexA + 1);
                if (indexB == -1) {
                    indexB = value.Length;
                }
            }

            return value.JSubstring(indexA + 1, indexB);
        }
    }
}

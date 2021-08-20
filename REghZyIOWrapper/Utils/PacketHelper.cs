using System;
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
    }
}

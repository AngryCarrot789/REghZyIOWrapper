using System;
using System.Text;

namespace REghZyIOWrapper.Utils {
    public static class PacketFormatting {
        /// <summary>
        /// Stretches the given string by placing the given character infront
        /// </summary>
        /// <param name="str">The value</param>
        /// <param name="targLen">The length of the string to return</param>
        /// <exception cref="Exception">Thrown if the length of the given value is larger than the target length</exception>
        public static string StretchFront(string str, int targLen, char front) {
            int actualLength = str.Length;
            if (actualLength == targLen) {
                return str;
            }
            else if (actualLength > targLen) {
                throw new Exception("String value's length was too big");
            }
            else {
                StringBuilder sb = new StringBuilder(targLen);
                for(int i = 0, targetCount = targLen - actualLength; i < targetCount; i++) {
                    sb.Append(front);
                }

                return sb.Append(str).ToString();
            }
        }

        public static string StretchEnd(string str, int targLen, char front) {
            int actualLength = str.Length;
            if (actualLength == targLen) {
                return str;
            }
            else if (actualLength > targLen) {
                throw new Exception("String value's length was too big");
            }
            else {
                StringBuilder sb = new StringBuilder(targLen).Append(str);
                for (int i = 0, targetCount = targLen - actualLength; i < targetCount; i++) {
                    sb.Append(front);
                }

                return sb.ToString();
            }
        }
    }
}

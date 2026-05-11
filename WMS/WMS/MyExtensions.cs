using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        /// 

        /// Returns the last few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the 
        /// given length the complete string is returned. If length is zero or 
        /// less an empty string is returned
        /// 

        /// the string to process/// Number of characters to return/// 
        public static string Right(this string s, int length)
        {
            length = Math.Max(length, 0);

            if (s.Length > length)
            {
                return s.Substring(s.Length - length, length);
            }
            else
            {
                return s;
            }
        }
        public static Byte[] notNulltimestamp(this Byte[] s)
        {
            if (s != null && s.Length > 0)
            {
                return s;
            }
            else
            {
                Byte[] a = new Byte[] { 0x0000000000000000 };
                return a;
            }
        }
    }
}
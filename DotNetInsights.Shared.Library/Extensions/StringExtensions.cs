using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class StringExtensions
    {
        public static string GetString(this IEnumerable<byte> byteValue, Encoding encoding)
        {
            return encoding.GetString(byteValue.ToArray());
        }

        public static byte[] GetBytes(this string value, Encoding encoding)
        {
            return encoding.GetBytes(value);
        }
    }
}

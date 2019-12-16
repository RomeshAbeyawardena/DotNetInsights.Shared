using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DotNetInsights.Shared.Library.Attributes
{
    public static class ObjectExtensions
    {
        public static byte[] GetBytes(this string value, Encoding encoding)
        {
            return encoding.GetBytes(value);
        }

        public static string GetString(this IEnumerable<byte> value, Encoding encoding)
        {
            return encoding.GetString(value.ToArray());
        }
    }
}

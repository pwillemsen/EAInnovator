using System;
using System.Globalization;

namespace EAInnovator.Helpers
{
    /// <summary>
    /// Helper class for standard Innovator to EA type conversions
    /// </summary>
    static class Tools
    {
        public static DateTime GetDate(string dateIn)
        {
            /// <summary>
            /// Convert database date/time string (e.g. 2019-10-31T01:00:00) to C# DateTime
            /// </summary>
            DateTime dateOut = DateTime.Parse(dateIn.Replace("T", " "), CultureInfo.InvariantCulture);
            return dateOut;
        }
    }
}
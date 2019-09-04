using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EAInnovator.Helpers
{
    static class Tools
    {
        public static string SanitizeIdentifier(string identifier)
        {
            if (identifier == null)
                return null;

            return Regex.Replace(identifier, "[^A-Za-z0-9_]", "_");

        }

        public static DateTime GetDate(string dateIn)
        {
            DateTime dateOut = DateTime.Parse(dateIn.Replace("T", " "), CultureInfo.InvariantCulture);
            return dateOut;
        }
    }
}
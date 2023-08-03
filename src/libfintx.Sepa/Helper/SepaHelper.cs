using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace libfintx.Sepa.Helper
{
    public static class SepaHelper
    {
        public static string Escape(string str)
        {
            // Zunächst XML-escapen
            var escaped = SecurityElement.Escape(str);
            // Dann sicherstellen, dass nur gemäß SEPA gültige Zeichen verwendet werden
            return ConvertToValidSepaString(escaped);
        }

        public static string ConvertToValidSepaString(string str)
        {
            return new string(str.Select(c => ConvertToValidSepaString(c)).SelectMany(s => s).ToArray());
        }

        /// <summary>
        /// Hier findet kein XML-Escaping statt, sondern es wird sichergestellt, dass nur die in SEPA gültigen Zeichen verwendet werden.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string ConvertToValidSepaString(char c)
        {
            switch (c)
            {
                case 'Ä': return "Ae";
                case 'Ö': return "Oe";
                case 'Ü': return "Ue";
                case 'ä': return "ae";
                case 'ö': return "oe";
                case 'ü': return "ue";
                case 'ß': return "ss";
            }

            if (!Regex.Match($"{c}", @"^[\sa-zA-Z0-9/?:\(\)\.,'+-]$").Success)
                return string.Empty;

            return $"{c}";
        }
    }
}
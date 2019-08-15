
// ReSharper disable StringIndexOfIsCultureSpecific.1
namespace KLineEdCmdApp.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Return the string slice between the two indexes.
        ///     Snip (2, 2) - return string containing just the character at index 2
        ///     Snip (1, 2) - return string containing the three character between index 0 and index 2
        /// </summary>
        public static string Snip(this string source, int startIndex, int endIndex)
        {
            string rc = null;
            if ((startIndex >= 0) && (endIndex >= 0) && (endIndex >= startIndex) && (endIndex < source.Length))
            {
                rc = source.Substring(startIndex, endIndex - startIndex + 1); // Return Substring of length
            }
            return rc;
        }

        public static string GetPropertyFromString(string source, string firstLabel, string secondLabel)
        {
            string rc = null;

            if ((source != null) && (firstLabel != null))
            {
                var startIndex = source.IndexOf(firstLabel);
                var endIndex = (secondLabel != null) ? source.IndexOf(secondLabel) - 1 : source.Length - 1;
                if ((startIndex >= 0) && (endIndex >= 0) && (endIndex >= startIndex + firstLabel.Length))
                {
                    startIndex += firstLabel.Length;

                    rc = source.Snip(startIndex, endIndex);
                }
            }
            return rc;
        }
    }
}

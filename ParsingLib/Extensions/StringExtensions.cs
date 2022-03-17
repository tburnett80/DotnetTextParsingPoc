
namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Simple null check for a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Null safe trim extension
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryTrim(this string value)
        {
            return value.HasValue()
                ? value.Trim()
                : value;
        }
    }
}

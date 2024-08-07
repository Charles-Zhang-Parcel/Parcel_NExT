﻿using System.Text;

namespace Parcel.CoreEngine.Helpers
{
    public static class StringHelper
    {
        #region Constants
        public static readonly char[] NewLineSeparators = ['\r', '\n'];
        public static string[] SplitLines(this string text) => text.Split(NewLineSeparators);
        public static string[] SplitLines(this string text, bool removeEmpty) => text.Split(NewLineSeparators, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        public static string[] SplitLines(this string text, bool removeEmpty, bool trimEntries) => text.Split(NewLineSeparators, (removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None) | (trimEntries ? StringSplitOptions.TrimEntries : StringSplitOptions.None));
        #endregion

        #region Original
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = (hash1 << 5) + hash1 ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = (hash2 << 5) + hash2 ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
        /// <summary>
        /// Works both on command line arguments and csv lines (without line escaping support)
        /// TODO: Is this functionally equivalent to <seealso cref="SplitCommandLine(string, char)"/>? If so, we should merge the two.
        /// </summary>
        /// <remarks>
        /// In some codes I authored before, this is also called "SplitArgumentsLikeCsv"
        /// </remarks>
        public static string[] SplitCommandLineArguments(this string inputString, char separator = ' ', bool includeQuotesInString = false)
        {
            List<string> parameters = [];
            StringBuilder current = new();

            bool inQuotes = false;
            foreach (var c in inputString)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    if (includeQuotesInString)
                        current.Append(c);
                }
                else if (c == separator)
                {
                    if (!inQuotes)
                    {
                        parameters.Add(current.ToString());
                        current.Clear();
                    }
                    else
                        current.Append(c);
                }
                else
                {
                    current.Append(c);
                }
            }
            if (current.Length != 0)
                parameters.Add(current.ToString());
            return [.. parameters];
        }
        public static string Camelize(this string original)
        {
            string stripSpaces = original.Replace(" ", string.Empty);
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(stripSpaces); // Notice JSON CamelCase conversion doesn't strip spaces by default
        }
        public static string Pascalize(this string original)
        {
            if (original.Length == 0)
                return original;
            
            string camel = Camelize(original);
            return new string([char.ToUpper(camel[0]), .. camel.Skip(1)]);
        }
        public static string JoinAsArguments(this string[] args)
        {
            if (args == null || args.Length == 0)
                return "";
            return string.Join(" ", args.Select(a => a.Contains('"') ? $"\"{a}\"" : a));
        }
        #endregion

        #region Command-Line String Parsing
        public static string[] SplitArgumentsLikeCsv(this string line, char separator = ',', bool ignoreEmptyEntries = false)
        {
            string[] arguments = line.SplitCommandLineArguments(separator);

            if (ignoreEmptyEntries)
                return arguments.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            else
                return arguments;
        }
        public static IEnumerable<string> SplitCSVLine(this string csvline)
            => SplitCommandLine(csvline, ',');
        public static IEnumerable<string> SplitCommandLine(this string commandLine, char delimiter = ' ')
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == delimiter;
            })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        private static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }
        private static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[^1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
        #endregion

        #region Tags
        public static string[] SplitTags(string csv, char splitter = ',')
            => csv.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower()).Distinct().OrderBy(t => t).ToArray();
        public static string DisplayTags(string[] itemTags) => string.Join(", ", itemTags);
        #endregion
    }
}

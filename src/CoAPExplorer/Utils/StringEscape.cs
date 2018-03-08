using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoAPExplorer.Utils
{
    public static class StringEscape
    {
        public static string Escape(string value)
        {
            Func<char, string> escapeChar = c =>
            {
                switch (c)
                {
                    case '\\': return @"\\";
                    case '\0': return @"\0";
                    case '\a': return @"\a";
                    case '\b': return @"\b";
                    case '\f': return @"\f";
                    case '\n': return @"\n";
                    case '\r': return @"\r";
                    case '\t': return @"\t";
                    case '\v': return @"\v";
                }
                if ((int)c <= 255)
                    return "\\x" + ((int)c).ToString("x2");
                return "\\u" + ((int)c).ToString("x4");
            };

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                char[] controlChars = new[] { '\\', '0', 'a', 'b', 'f', 'l', 'n', 'r', 't', 'v', 'x', 'u', 'U' };

                if (char.IsControl(c) && !char.IsWhiteSpace(c))
                    sb.Append(escapeChar(c));
                else if (c == '\\' && controlChars.Contains(value[i + 1]))
                    sb.Append(@"\\");
                else if (char.IsSurrogatePair(value, i))
                    sb.Append("\\U" + char.ConvertToUtf32(value, i++).ToString("x8"));
                else if (c > 127)
                    sb.Append("\\u" + ((int)c).ToString("x4"));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string Unescape(string value)
        {
            return Regex.Replace(value, @"\\(?:(\\|0|a|b|f|n|r|t|v)|U([a-fA-F0-9]{8})|u([a-fA-F0-9]{4})|x([a-fA-F0-9]{1,4}))",
                matches => {
                    string match = matches.Groups[1].Success ? matches.Groups[1].Value // control characters
                                 : matches.Groups[2].Success ? matches.Groups[2].Value // 8-digit unicode sequence
                                 : matches.Groups[3].Success ? matches.Groups[3].Value // 4-digit unicdoe sequence
                                 : matches.Groups[4].Success ? matches.Groups[4].Value // 1 to 4-digit hex sequence
                                 : throw new NotImplementedException($"Regex issue in {nameof(StringEscape)}");

                    if (matches.Groups[1].Success)
                    {
                        switch (match)
                        {
                            case @"\": return "\\";
                            case @"0": return "\0";
                            case @"a": return "\a";
                            case @"b": return "\b";
                            case @"f": return "\f";
                            case @"n": return "\n";
                            case @"r": return "\r";
                            case @"t": return "\t";
                            case @"v": return "\v";
                            default:
                                throw new NotImplementedException($"Unsupported escape character ({match})");
                        }
                    }
                    return (char.ConvertFromUtf32(int.Parse(match, NumberStyles.HexNumber)));
                });
        }
    }
}

using System;
using System.Globalization;
using System.Text;

namespace Bannerlord.ModuleManager.Native
{
    internal static class Utils
    {
        public static string UnescapeString(ReadOnlySpan<char> unescapedStr)
        {
            var sb = new StringBuilder(32);
            var q = false;
            Span<char> hexb = stackalloc char[4];
            for (var i = 0; i < unescapedStr.Length; i++)
            {
                var chr = unescapedStr[i];
                if (!q)
                {
                    if (chr == '\\')
                    {
                        q = true;
                        continue;
                    }
                    sb.Append(chr);
                }
                else
                {
                    switch (chr)
                    {
                        case 'u':
                        case 'U':
                            if (unescapedStr.Length - i < 4) throw new Exception($"Not enough length for the unicode escape symbol");
                            unescapedStr.Slice(i + 1, 4).CopyTo(hexb);
                            chr = (char) ushort.Parse(hexb, NumberStyles.HexNumber);
                            sb.Append(chr);
                            i += 4;
                            break;
                        default: throw new Exception($"Invalid backslash escape (\\ + charcode {(int) chr})");
                    }
                    q = false;
                }
            }
            return sb.ToString();
        }
    }
}
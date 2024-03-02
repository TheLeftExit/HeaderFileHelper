using System.Text.RegularExpressions;

public static partial class HeaderFileHelper
{
    [GeneratedRegex(@"\ *//.*?(?=\r\n)", RegexOptions.Multiline)]
    private static partial Regex SingleLineComments();
    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline | RegexOptions.Multiline)]
    private static partial Regex MultiLineComments();
    [GeneratedRegex(@"(?<=^)#.*?[^\\](?=\r\n)", RegexOptions.Singleline | RegexOptions.Multiline)]
    private static partial Regex Directives();
    [GeneratedRegex(@"[\\\s]+", RegexOptions.Multiline)]
    private static partial Regex WhitespaceCharacters();

    public static string WithoutComments(this string text)
    {
        text = SingleLineComments().Replace(text, "");
        text = MultiLineComments().Replace(text, "");
        return text;
    }

    public static IEnumerable<string> GetDirectives(this string text)
    {
        return Directives().Matches(text).Select(x => x.Value);
    }

    public static string RemoveDirectives(this string text)
    {
        return Directives().Replace(text, "");
    }

    public static IEnumerable<string> GetInstructions(this string text)
    {
        var instructionText = text.RemoveDirectives();
        var semicolonIndexes = new List<int>();
        var depth = 0;
        var maxDepth = 0;
        for (int i = 0; i < instructionText.Length; i++)
        {
            if (instructionText[i] == '{')
            {
                if (++depth == maxDepth) semicolonIndexes.Add(i);
            }
            else if (instructionText[i] == '}')
            {
                if (--depth == maxDepth - 1) semicolonIndexes.Add(i);
            }
            else if (instructionText[i] == ';' && depth <= maxDepth)
            {
                semicolonIndexes.Add(i);
            }
        }
        return Enumerable
            .Range(0, Math.Max(0, semicolonIndexes.Count - 2))
            .Select(x => instructionText[(semicolonIndexes[x] + 1)..(semicolonIndexes[x + 1] + 1)]);
    }

    public static IEnumerable<string> CleanUp(this IEnumerable<string> lines)
    {
        return lines
            .Select(x => WhitespaceCharacters().Replace(x, " ").Trim())
            .Where(x => !string.IsNullOrEmpty(x));
    }
}
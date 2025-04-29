using System.Text.RegularExpressions;

namespace dotnetCampus.Logger.Utils.CodeAnalysis;

/// <summary>
/// 为通过模板生成的源代码提供正则表达式。
/// </summary>
public static class TemplateRegexes
{
    private static Regex? _namespaceRegex;
    private static Regex? _typeModifierRegex;
    private static Regex? _flagRegex;
    private static Regex? _flag1Regex;
    private static Regex? _flag2Regex;
    private static Regex? _flag3Regex;
    private static Regex? _flag4Regex;
    private static Regex NamespaceRegex => _namespaceRegex ??= new Regex(@"\bnamespace\s+([\w\.]+)(?=\s*[;\{])", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex TypeModifierRegex => _typeModifierRegex ??= new Regex(@"\b(public|internal)(?=[\s\w_]+(class|record|struct|enum|interface)\s[\w_]+)", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex FlagRegex => _flagRegex ??= new Regex(@"(?<=\n)\s+// <FLAG>.+?</FLAG>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex Flag1Regex => _flag1Regex ??= new Regex(@"(?<=\n)\s+// <FLAG1>.+?</FLAG1>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex Flag2Regex => _flag2Regex ??= new Regex(@"(?<=\n)\s+// <FLAG2>.+?</FLAG2>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex Flag3Regex => _flag3Regex ??= new Regex(@"(?<=\n)\s+// <FLAG3>.+?</FLAG3>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static Regex Flag4Regex => _flag4Regex ??= new Regex(@"(?<=\n)\s+// <FLAG4>.+?</FLAG4>", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// 替换代码中的命名空间声明。
    /// </summary>
    /// <param name="content">包含要替换命名空间代码的字符串。</param>
    /// <param name="newNamespace">新的命名空间。</param>
    /// <returns>替换了命名空间的新代码。</returns>
    public static string ReplaceNamespace(this string content, string newNamespace)
    {
        return NamespaceRegex.Replace(content, $"namespace {newNamespace}");
    }

    /// <summary>
    /// 替换代码中的类型访问修饰符。
    /// </summary>
    /// <param name="content">包含要替换类型访问修饰符的字符串。</param>
    /// <param name="newModifier">新的类型访问修饰符。</param>
    /// <returns>替换了类型访问修饰符的新代码。</returns>
    public static string ReplaceTypeModifier(this string content, string newModifier)
    {
        return TypeModifierRegex.Replace(content, newModifier);
    }

    /// <summary>
    /// 替换代码中的 // <FLAG>...</FLAG> 注释，将其替换为指定的内容。
    /// </summary>
    /// <param name="content">包含要替换的代码的字符串。</param>
    /// <param name="flagContent">要替换的内容。</param>
    /// <returns>替换后的字符串。</returns>
    public static string FlagReplace(this string content, string flagContent)
    {
        return FlagRegex.Replace(content, flagContent);
    }

    /// <summary>
    /// 替换代码中的 // <FLAG1>...</FLAG1> 注释，将其替换为指定的内容。
    /// </summary>
    /// <param name="content">包含要替换的代码的字符串。</param>
    /// <param name="flagContent">要替换的内容。</param>
    /// <returns>替换后的字符串。</returns>
    public static string Flag1Replace(this string content, string flagContent)
    {
        return Flag1Regex.Replace(content, flagContent);
    }

    /// <summary>
    /// 替换代码中的 // <FLAG2>...</FLAG2> 注释，将其替换为指定的内容。
    /// </summary>
    /// <param name="content">包含要替换的代码的字符串。</param>
    /// <param name="flagContent">要替换的内容。</param>
    /// <returns>替换后的字符串。</returns>
    public static string Flag2Replace(this string content, string flagContent)
    {
        return Flag2Regex.Replace(content, flagContent);
    }

    /// <summary>
    /// 替换代码中的 // <FLAG3>...</FLAG3> 注释，将其替换为指定的内容。
    /// </summary>
    /// <param name="content">包含要替换的代码的字符串。</param>
    /// <param name="flagContent">要替换的内容。</param>
    /// <returns>替换后的字符串。</returns>
    public static string Flag3Replace(this string content, string flagContent)
    {
        return Flag3Regex.Replace(content, flagContent);
    }

    /// <summary>
    /// 替换代码中的 // <FLAG4>...</FLAG4> 注释，将其替换为指定的内容。
    /// </summary>
    /// <param name="content">包含要替换的代码的字符串。</param>
    /// <param name="flagContent">要替换的内容。</param>
    /// <returns>替换后的字符串。</returns>
    public static string Flag4Replace(this string content, string flagContent)
    {
        return Flag4Regex.Replace(content, flagContent);
    }
}

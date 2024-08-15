using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnetCampus.Logging.Writers.Helpers;

internal class TagFilterManager
{
    public const string LogTagParameterName = "--log-console-tags";

    /// <summary>
    /// 当前已设置的任一标签。（无前缀）
    /// </summary>
    public required ImmutableHashSetString AnyFilterTags { get; init; }

    /// <summary>
    /// 当前已设置的包含标签。（前缀为 +）
    /// </summary>
    public required ImmutableHashSetString IncludingFilterTags { get; init; }

    /// <summary>
    /// 当前已设置的排除标签。（前缀为 -）
    /// </summary>
    public required ImmutableHashSetString ExcludingFilterTags { get; init; }

    /// <summary>
    /// 判断某个日志是否满足当前标签过滤条件。
    /// </summary>
    /// <param name="text">要判断的日志原文。</param>
    /// <returns>是否满足过滤条件。</returns>
    /// <remarks>
    /// 匹配原则：
    /// <list type="number">
    /// <item>先看任一标签进行初筛：只要有一个标签匹配，即选出；但如果没有指定任一标签，则全部选出。</item>
    /// <item>在前一个初筛的基础上，再看排除标签：只要有一个标签匹配，即排除。</item>
    /// <item>在前两个筛选的基础上，再看包含标签：必须全部标签匹配，才选出，其他全部排除。</item>
    /// </list>
    /// </remarks>
    internal bool IsTagEnabled(string text)
    {
        if (AnyFilterTags.Count is 0 && ExcludingFilterTags.Count is 0 && IncludingFilterTags.Count is 0)
        {
            return true;
        }

        var defaultEnabled = AnyFilterTags.Count is 0 && IncludingFilterTags.Count is 0;
        var currentTagStartIndex = -1;
        var isInTag = false;
        var matchAny = false;
        List<string> includingTags = IncludingFilterTags.ToList();
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '[')
            {
                // 进入标签。
                currentTagStartIndex = i;
                isInTag = true;
            }
            else if (text[i] == ']')
            {
                // 离开标签。
                var currentTagEndIndex = i;
                isInTag = false;
                if (currentTagStartIndex < 0)
                {
                    return defaultEnabled;
                }
                var tag = text.AsSpan().Slice(currentTagStartIndex + 1, currentTagEndIndex - currentTagStartIndex - 1).ToString();
                // 只要有一个排除标签匹配，就不输出。
                if (ExcludingFilterTags.Contains(tag))
                {
                    return false;
                }
                // 如果有任一标签，则匹配一个即可。
                matchAny = matchAny || defaultEnabled || AnyFilterTags.Contains(tag);
                if (matchAny)
                {
                    // 如果有包含标签，则匹配一个，直到全部匹配。
                    if (IncludingFilterTags.Count > 0)
                    {
                        if (includingTags.Contains(tag))
                        {
                            includingTags.Remove(tag);
                        }
                        if (includingTags.Count is 0)
                        {
                            return true;
                        }
                        continue;
                    }

                    if (ExcludingFilterTags.Count is 0)
                    {
                        return true;
                    }
                }
            }
            else if (char.IsWhiteSpace(text[i]))
            {
                // 空白字符，不处理。
            }
            else if (!isInTag)
            {
                // 当前不在标签内，且非空白字符，直接跳出。
                return defaultEnabled || matchAny;
            }
        }
        return defaultEnabled || matchAny;
    }

    /// <summary>
    /// 从命令行参数中提取过滤标签。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    public static TagFilterManager? FromCommandLineArgs(string[] args)
    {
        HashSet<string> anyFilterTags = [];
        HashSet<string> includingFilterTags = [];
        HashSet<string> excludingFilterTags = [];
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] != LogTagParameterName || i + 1 >= args.Length)
            {
                continue;
            }

            var filterTags = args[i + 1].Split([',', ';', ' ']);
            foreach (var tag in filterTags)
            {
                if (tag.StartsWith("-", StringComparison.Ordinal))
                {
#if NET8_0_OR_GREATER
                    excludingFilterTags.Add(tag[1..]);
#else
                    excludingFilterTags.Add(tag.Substring(1));
#endif
                }
                else if (tag.StartsWith("+", StringComparison.Ordinal))
                {
#if NET8_0_OR_GREATER
                    includingFilterTags.Add(tag[1..]);
#else
                    includingFilterTags.Add(tag.Substring(1));
#endif
                }
                else
                {
                    anyFilterTags.Add(tag);
                }
            }

            return new TagFilterManager
            {
                AnyFilterTags = anyFilterTags.ToImmutableHashSet(),
                IncludingFilterTags = includingFilterTags.ToImmutableHashSet(),
                ExcludingFilterTags = excludingFilterTags.ToImmutableHashSet(),
            };
        }

        return new TagFilterManager
        {
            AnyFilterTags = [],
            IncludingFilterTags = [],
            ExcludingFilterTags = [],
        };
    }
}

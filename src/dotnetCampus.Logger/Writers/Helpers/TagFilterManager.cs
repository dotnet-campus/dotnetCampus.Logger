using System;
using System.Collections.Generic;

namespace dotnetCampus.Logging.Writers.Helpers;

internal class TagFilterManager
{
    /// <summary>
    /// 当前已设置的过滤标签。（无前缀，代表任一匹配。）
    /// </summary>
    public required ImmutableHashSetString AnyFilterTags { get; init; }

    /// <summary>
    /// 当前已设置的过滤标签。（前缀为 +，代表必须匹配。）
    /// </summary>
    public required ImmutableHashSetString IncludingFilterTags { get; init; }

    /// <summary>
    /// 当前已设置的过滤标签。（前缀为 -，代表必须不匹配。）
    /// </summary>
    public required ImmutableHashSetString ExcludingFilterTags { get; init; }

    /// <summary>
    /// 判断某个日志是否满足当前标签过滤条件。
    /// </summary>
    /// <param name="text">要判断的日志原文。</param>
    /// <returns>是否满足过滤条件。</returns>
    internal bool IsTagEnabled(string text)
    {
        if (AnyFilterTags.Count is 0 && ExcludingFilterTags.Count is 0 && IncludingFilterTags.Count is 0)
        {
            return true;
        }

        var defaultEnabled = AnyFilterTags.Count is 0 && IncludingFilterTags.Count is 0;
        var currentTagStartIndex = -1;
        var isInTag = false;
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

            }
            else if (char.IsWhiteSpace(text[i]))
            {
                // 空白字符，不处理。
            }
            else if (!isInTag)
            {
                // 当前不在标签内，且非空白字符，直接跳出。
                return defaultEnabled;
            }
        }
        return defaultEnabled;
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
            if (args[i] != "--log-console-tags" || i + 1 >= args.Length)
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

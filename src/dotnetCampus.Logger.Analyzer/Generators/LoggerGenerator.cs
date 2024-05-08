﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotnetCampus.Logger.Utils.IO;
using dotnetCampus.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Logger.Generators;

/// <summary>
/// 生成一组用于记录日志的代码。
/// </summary>
[Generator]
public class LoggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.AnalyzerConfigOptionsProvider;
        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider provider)
    {
        if (!provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace))
        {
            return;
        }

        foreach (var file in EmbeddedSourceFiles.Enumerate("Assets/Sources"))
        {
            var code = GenerateSource(rootNamespace, file.Content);
            context.AddSource(file.FileName, SourceText.From(code, Encoding.UTF8));
        }
    }

    private string GenerateSource(string rootNamespace, string sourceText)
    {
        var sourceSpan = sourceText.AsSpan();

        var namespaceKeywordIndex = sourceText.IndexOf("namespace", StringComparison.Ordinal);
        var namespaceStartIndex = namespaceKeywordIndex + "namespace".Length + 1;
        var namespaceEndIndex = sourceText.IndexOf(";", namespaceStartIndex, StringComparison.Ordinal);

        var @namespace = sourceSpan.Slice(namespaceStartIndex, namespaceEndIndex - namespaceStartIndex).Trim();

        // 搜索 class/record/struct/enum/interface 等关键字
        var classKeywordIndex = GetTypeRegex().Match(sourceText).Index;
        var publicKeywordIndex = sourceText.IndexOf("public", namespaceEndIndex, classKeywordIndex - namespaceEndIndex, StringComparison.Ordinal);

        return string.Concat(
            sourceSpan.Slice(0, namespaceStartIndex).ToString(),
            rootNamespace,
            sourceSpan.Slice(namespaceEndIndex, publicKeywordIndex - namespaceEndIndex).ToString(),
            "internal",
            sourceSpan.Slice(publicKeywordIndex + "public".Length, sourceSpan.Length - publicKeywordIndex - "public".Length).ToString()
        );
    }

    private static Regex? _typeRegex;

    private static Regex GetTypeRegex() => _typeRegex ??= new Regex(@"\b(?:class|record|struct|enum|interface)\b", RegexOptions.Compiled);
}

﻿using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using dotnetCampus.Logger.Utils.CodeAnalysis;
using dotnetCampus.Logger.Utils.IO;
using Microsoft.CodeAnalysis;
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
        if (provider.GlobalOptions
                .TryGetValue<string>("_DLRootNamespace", out var rootNamespace)
                .TryGetValue<bool>("_DLGenerateSource", out var isGenerateSource)
                is var result
                && !result)
        {
            // 此项目是通过依赖间接引用的，没有 build 因此无法在源生成器中使用编译属性，所以只能选择引用。
            return;
        }

        if (!isGenerateSource)
        {
            return;
        }

        var sourceFiles = EmbeddedSourceFiles.Enumerate("Assets/Sources").ToImmutableArray();

        foreach (var file in sourceFiles)
        {
            var code = GenerateSource(file.TypeName, rootNamespace, file.Content);
            context.AddSource($"{file.TypeName}.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }

    private string GenerateSource(string typeName, string rootNamespace, string sourceText)
    {
        if (typeName == "Log")
        {
            // 源生成器为单独库生成的代码中，默认日志记录器是 BridgeLogger。
            sourceText = sourceText.Replace("new MemoryCacheLogger();", "new BridgeLogger();");
        }

        var sourceSpan = sourceText.AsSpan();

        var namespaceKeywordIndex = sourceText.IndexOf("namespace", StringComparison.Ordinal);
        var namespaceStartIndex = namespaceKeywordIndex + "namespace".Length + 1;
        var namespaceEndIndex = sourceText.IndexOf(";", namespaceStartIndex, StringComparison.Ordinal);

        var classKeywordIndex = GetTypeRegex().Match(sourceText).Index;
        var publicKeywordIndex = sourceText.IndexOf("public", namespaceEndIndex, classKeywordIndex - namespaceEndIndex, StringComparison.Ordinal);

        if (publicKeywordIndex < 0 || typeName.Contains("Bridge"))
        {
            // 此类型不是 public 的，无需修改为 internal。
            // 此类型是 BridgeLogger，应该保持 public。
            return string.Concat(
                sourceSpan.Slice(0, namespaceStartIndex).ToString(),
                $"{rootNamespace}.Logging",
                sourceSpan.Slice(namespaceEndIndex, sourceSpan.Length - namespaceEndIndex).ToString()
            );
        }
        else
        {
            // 此类型是 public 的，需要修改为 internal。
            return string.Concat(
                sourceSpan.Slice(0, namespaceStartIndex).ToString(),
                $"{rootNamespace}.Logging",
                sourceSpan.Slice(namespaceEndIndex, publicKeywordIndex - namespaceEndIndex).ToString(),
                "internal",
                sourceSpan.Slice(publicKeywordIndex + "public".Length, sourceSpan.Length - publicKeywordIndex - "public".Length).ToString()
            );
        }
    }

    private static Regex? _typeRegex;

    private static Regex GetTypeRegex() => _typeRegex ??= new Regex(@"\b(?:class|record|struct|enum|interface)\b", RegexOptions.Compiled);
}

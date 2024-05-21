using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotnetCampus.Logger.Assets.Templates;
using dotnetCampus.Logging.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Logger.Generators;

/// <summary>
/// 生成聚合日志桥，为来自各个库的日志桥对接日志记录器。
/// </summary>
[Generator]
public class LoggerBridgeGenerator : IIncrementalGenerator
{
    private static readonly string ImportLoggerBridgeUsageName = nameof(ImportLoggerBridgeAttribute).Replace("Attribute", "");
    private static readonly string ImportLoggerBridgeAttributeName = typeof(ImportLoggerBridgeAttribute).FullName!;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider((node, ct) =>
            {
                if (node is not ClassDeclarationSyntax cds)
                {
                    // 必须是类声明。
                    return false;
                }

                var attributes = cds.AttributeLists
                    .SelectMany(x => x.Attributes)
                    .Where(x => x.Name.ToString().StartsWith(ImportLoggerBridgeUsageName));
                if (!attributes.Any())
                {
                    // 必须有 ImportLoggerBridge 特性。
                    return false;
                }

                return true;
            }, (c, ct) =>
            {
                var aggregateBridgeType = c.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)c.Node);
                if (aggregateBridgeType is null)
                {
                    return null;
                }

                var collectedBridgeTypes = aggregateBridgeType.GetAttributes()
                    .Where(x => x.AttributeClass switch
                    {
                        { IsGenericType: true } ga => ga.OriginalDefinition.ToDisplayString() == $"{ImportLoggerBridgeAttributeName}<T>",
                        _ => x.AttributeClass?.ToDisplayString() == ImportLoggerBridgeAttributeName,
                    })
                    .Select(x => x.AttributeClass switch
                    {
                        { IsGenericType: true } ga => ga.TypeArguments.FirstOrDefault() as INamedTypeSymbol,
                        var ba => x.ConstructorArguments.FirstOrDefault().Value as INamedTypeSymbol,
                    })
                    .OfType<INamedTypeSymbol>()
                    .ToImmutableArray();
                if (collectedBridgeTypes.Length is 0)
                {
                    return null;
                }

                return new LoggerBridgeItem(aggregateBridgeType, collectedBridgeTypes);
            })
            .Where(x => x is not null)
            .Select((x, ct) => x!);

        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, LoggerBridgeItem bridgeItem)
    {
        var bridgeNamespace = bridgeItem.Aggregate.ContainingNamespace.ToDisplayString();
        var bridgeName = bridgeItem.Aggregate.Name;

        var loggerBridgeFile = GeneratorInfo.GetEmbeddedTemplateFile<AggregateLoggerBridgeLinker>();
        var intermediateCode = loggerBridgeFile.Content
            .Replace(typeof(AggregateLoggerBridgeLinker).Namespace!, bridgeNamespace)
            .Replace(nameof(AggregateLoggerBridgeLinker), bridgeName)
            .Replace(
                $"partial class {bridgeName} : GILoggerBridgeLinker",
                $"partial class {bridgeName} : GILoggerBridgeLinker{string.Concat(bridgeItem.Collected.Select(x => $",\n    global::{x.ToDisplayString()}"))}");

        var generatedCode = InsertLinks(bridgeItem, intermediateCode);

        context.AddSource($"{nameof(AggregateLoggerBridgeLinker)}.{bridgeName}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
    }

    private string InsertLinks(LoggerBridgeItem bridgeItem, string sourceCode)
    {
        var sourceSpan = sourceCode.AsSpan();

        var regex = GetFlagRegex();
        var match = regex.Match(sourceCode);
        if (!match.Success)
        {
            return sourceCode;
        }

        var insertStartIndex = match.Index;
        var insertEndIndex = match.Index + match.Length;
        var links = GenerateLinks(bridgeItem.Collected);

        return string.Concat(
            sourceSpan.Slice(0, insertStartIndex).ToString(),
            links,
            sourceSpan.Slice(insertEndIndex, sourceSpan.Length - insertEndIndex).ToString()
        );
    }

    private string GenerateLinks(IEnumerable<INamedTypeSymbol> collected)
    {
        return $"""

{string.Join("\n", collected.Select(x => $"        {GenerateLink(x)}"))}
""";
    }

    private string GenerateLink(INamedTypeSymbol collectedBridgeType)
    {
        // global::Xxx.Logging.ILoggerBridge.Link(this);
        return $"global::{collectedBridgeType.ToDisplayString()}.Link(this);";
    }

    private static Regex? _flagRegex;

    private static Regex GetFlagRegex() => _flagRegex ??= new Regex(@"\s+// <FLAG>.+?</FLAG>", RegexOptions.Compiled | RegexOptions.Singleline);

    private record LoggerBridgeItem(INamedTypeSymbol Aggregate, ImmutableArray<INamedTypeSymbol> Collected);
}

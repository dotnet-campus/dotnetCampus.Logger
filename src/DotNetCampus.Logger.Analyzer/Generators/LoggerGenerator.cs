using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetCampus.Logger.Utils.CodeAnalysis;
using DotNetCampus.Logger.Utils.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace DotNetCampus.Logger.Generators;

/// <summary>
/// 生成一组用于记录日志的代码。
/// </summary>
[Generator]
public class LoggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var analyzerConfigOptionsProvider = context.AnalyzerConfigOptionsProvider;
        var compilationProvider = context.CompilationProvider;

        context.RegisterSourceOutput(analyzerConfigOptionsProvider.Combine(compilationProvider), Execute);
    }

    private void Execute(SourceProductionContext context, (AnalyzerConfigOptionsProvider Left, Compilation Right) args)
    {
        var (provider, compilation) = args;
        if (provider.GlobalOptions
                .TryGetValue<string>("_DLRootNamespace", out var rootNamespace)
                .TryGetValue<bool>("DCUseGeneratedLogger", out var useGeneratedLogger)
                is var result
                && !result)
        {
            // 此项目未设置必要的属性（通常这是不应该出现的，因为 buildTransitive 传递的编译目标会自动生成这些属性）。
            return;
        }

        // 查找所有的 Log 类型（这只是一个代表，用于检测是否有引用的程序集包含即将生成的类型）。
        var logTypes = compilation.GetTypesByMetadataName("DotNetCampus.Logging.Log")
            .Where(x => x.DeclaredAccessibility is Accessibility.Public || x.ContainingAssembly.GivesAccessTo(compilation.Assembly))
            .ToArray();
        // 查找 Log 类型所在的所有程序集。
        var assemblies = logTypes
            .Select(x => x.ContainingAssembly.Name)
            .ToArray();
        // 检查 Log 类型的来源。
        var logVisibleType = (assemblies, logTypes.Length) switch
        {
            (["DotNetCampus.Logger"], _) => LogVisibleType.ReferenceCompile,
            (_, 0) => LogVisibleType.Invisible,
            (_, 1) => LogVisibleType.InternalsVisibleToSource,
            (_, _) when assemblies.Contains("DotNetCampus.Logger") => LogVisibleType.ConflictBetweenReferenceCompileAndInternalsVisibleToSource,
            (_, _) => LogVisibleType.ConflictAmongInternalsVisibleToSources,
        };

        foreach (var file in EmbeddedSourceFiles.Enumerate("Assets/Sources"))
        {
            var code = GenerateSource(file.TypeName, rootNamespace, file.Content);
            var dependentCode = WrapSource(file, code, logVisibleType, assemblies);
            context.AddSource($"{file.TypeName}.g.cs", SourceText.From(dependentCode, Encoding.UTF8));
        }
    }

    private string WrapSource(EmbeddedSourceFile file, string code, LogVisibleType visibleType, IReadOnlyList<string> assemblies)
    {
        var isLoggerBridge = file.TypeName is "ILoggerBridge";
        return visibleType switch
        {
            LogVisibleType.Invisible => code,
            LogVisibleType.ReferenceCompile => $"""
// 因为已引用了 DotNetCampus.Logger，所以不会生成 {file.TypeName} 类型。
// 如果希望使用源生成器，从而不额外引入依赖，请按如下方法设置你的包引用：
//
//   <PackageReference Include="DotNetCampus.Logger" PrivateAssets="all" ExcludeAssets="compile;runtime" />
//
// 其中：
//  1. PrivateAssets="all" 用于表明生成的 NuGet 包将不引入 DotNetCampus.Logger 包依赖；
//  2. ExcludeAssets 中 compile 用于不引入编译时的依赖（此时以下源才会自动解除注释）；
//  3. ExcludeAssets 中 runtime 用于不将依赖复制到目标目录（因为运行时已经不需要它们了）。

/*
{code}*/

""",
            LogVisibleType.InternalsVisibleToSource => isLoggerBridge
                ? $"""
// 因为日志类型已通过此项目的 InternalsVisibleTo 传递给了该项目，所以只生成 {file.TypeName} 这一个类型用于桥接。
{string.Join("\n", assemblies.Select(x => $"//  - {x} (通过 InternalsVisibleTo 传递)"))}

{code}
"""
                : $"""
// 因为日志类型已通过此项目的 InternalsVisibleTo 传递给了该项目，所以不会生成 {file.TypeName} 类型。
{string.Join("\n", assemblies.Select(x => $"//  - {x} (通过 InternalsVisibleTo 传递)"))}

/*
{code}*/

""",
            LogVisibleType.ConflictAmongInternalsVisibleToSources => isLoggerBridge
                ? $"""
// 日志类型已通过以下多个项目的 InternalsVisibleTo 传递给了该项目，生成 {file.TypeName} 类型用于桥接。
{string.Join("\n", assemblies.Select(x => $"//  - {x} (通过 InternalsVisibleTo 传递)"))}

{code}
"""
                : $"""
// 日志类型已通过以下多个项目的 InternalsVisibleTo 传递给了该项目，出现了类型冲突，所以会强制生成 {file.TypeName} 类型以解决冲突。
{string.Join("\n", assemblies.Select(x => $"//  - {x} (通过 InternalsVisibleTo 传递)"))}

// 如果希望消除编译时的冲突警告，请在项目中添加以下属性：
//
//   <PropertyGroup>
//     <!-- 类型冲突，将强制使用本项目中生成的源。 -->
//     <NoWarn>$(NoWarn);CS0436</NoWarn>
//   </PropertyGroup>

#pragma warning disable CS0436

{code}
""",
            LogVisibleType.ConflictBetweenReferenceCompileAndInternalsVisibleToSource => $"""
// # 类型冲突，项目无法编译
//
// ## 原因
//
// 同时引用了日志库和包含日志源生成器且设置了 InternalsVisibleTo 的项目：
{string.Join("\n", assemblies.Select(x => x == "DotNetCampus.Logger" ? $"//  - {x} (引用)" : $"//  - {x} (通过 InternalsVisibleTo 传递)"))}
//
// ## 解决方法
//
// 任选其一：
//
// 1. 全部使用引用，而不是源生成器。
//    即删除 ExcludeAssets="compile;runtime"
//    <PackageReference Include="DotNetCampus.Logger" />
// 2. 删除到此项目的 InternalsVisibleTo

/*
{code}*/

""",
            _ => throw new ArgumentOutOfRangeException(nameof(visibleType), visibleType, null),
        };
    }

    private string GenerateSource(string typeName, string rootNamespace, string sourceText)
    {
        if (typeName == "Log")
        {
            // 源生成器为单独库生成的代码中，默认日志记录器是 BridgeLogger。
            sourceText = sourceText.Replace("new MemoryCacheLogger();", "new BridgeLogger();");
        }

        if (typeName == "ILoggerBridge")
        {
            // 此类型是 ILoggerBridge，应该修改为 public（引用不公开，源要公开），且变更命名空间。
            return sourceText.ReplaceTypeModifier("public").ReplaceNamespace($"{rootNamespace}.Logging");
        }

        if (typeName == "BridgeLogger")
        {
            // 此类型是 BridgeLogger，里面用到了 ILoggerBridge，要使用新的命名空间。
            return sourceText.Replace("ILoggerBridge", $"global::{rootNamespace}.Logging.ILoggerBridge");

        }

        // 此类型是 public 的，需要修改为 internal。
        return sourceText.ReplaceTypeModifier("internal");
    }

    private enum LogVisibleType
    {
        /// <summary>
        /// 日志类型未引用，或不可见。
        /// </summary>
        Invisible,

        /// <summary>
        /// 日志类型通过库直接引用。
        /// </summary>
        ReferenceCompile,

        /// <summary>
        /// 日志类型在引用的项目中已生成，并通过 InternalsVisibleTo 传递了进来。
        /// </summary>
        InternalsVisibleToSource,

        /// <summary>
        /// 日志类型通过多个项目的 InternalsVisibleTo 传递了进来。
        /// </summary>
        ConflictAmongInternalsVisibleToSources,

        /// <summary>
        /// 日志类型既通过库直接引用，又通过 InternalsVisibleTo 传递了进来；此时必然会冲突，导致编译失败。
        /// </summary>
        ConflictBetweenReferenceCompileAndInternalsVisibleToSource,
    }
}

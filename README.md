# DotNetCampus.Logger

| Build | NuGet                                                                                                             |
|--|-------------------------------------------------------------------------------------------------------------------|
|![](https://github.com/dotnet-campus/DotNetCampus.Logger/workflows/.NET%20Core/badge.svg)| [![](https://img.shields.io/nuget/v/DotNetCampus.Logger.svg)](https://www.nuget.org/packages/DotNetCampus.Logger) |

## 入门

### 安装日志库

在你的项目中安装 `DotNetCampus.Logger` 包，你可以通过 NuGet 包管理器或者通过命令行来安装：

```shell
dotnet add package DotNetCampus.Logger
```

### 初始化

在你的初始化代码中添加如下代码，即可完成日志的配置和初始化：

```csharp
new LoggerBuilder()
    .WithLevel(LogLevel.Debug)
    .AddWriter(new ConsoleLogger())
    .Build()
    .IntoGlobalStaticLog();
```

### 使用

在使用上述方法完成初始化之后，你就可以在任何地方使用 `Log` 类来输出日志了：

```csharp
Log.Info("Hello, world!");
```

`Log` 静态类提供了多种方法来输出日志：

```csharp
// 这些日志仅在代码开启了 TRACE 条件编译符的情况下才会编译。
Log.TraceLogger.Trace("[SourceReference] Log.Trace.Trace");
Log.TraceLogger.Debug("[SourceReference] Log.Trace.Debug");
Log.TraceLogger.Info("[SourceReference] Log.Trace.Info");
Log.TraceLogger.Warn("[SourceReference] Log.Trace.Warn");
Log.TraceLogger.Error("[SourceReference] Log.Trace.Error");
Log.TraceLogger.Fatal("[SourceReference] Log.Trace.Fatal");

// 这些日志仅在代码开启了 DEBUG 条件编译符的情况下才会编译。
Log.DebugLogger.Trace("[SourceReference] Log.Debug.Trace");
Log.DebugLogger.Debug("[SourceReference] Log.Debug.Debug");
Log.DebugLogger.Info("[SourceReference] Log.Debug.Info");
Log.DebugLogger.Warn("[SourceReference] Log.Debug.Warn");
Log.DebugLogger.Error("[SourceReference] Log.Debug.Error");
Log.DebugLogger.Fatal("[SourceReference] Log.Debug.Fatal");

// 这些日志在任何情况下都会被编译。
Log.Trace("[SourceReference] Log.Trace");
Log.Debug("[SourceReference] Log.Debug");
Log.Info("[SourceReference] Log.Info");
Log.Warn("[SourceReference] Log.Warn");
Log.Error("[SourceReference] Log.Error");
Log.Fatal("[SourceReference] Log.Fatal");

// 这些方法与上面的方法等价。
// 主要用途为你可以通过 Log.Current 拿到一个 ILogger 的实例，用来接入项目的其他部分。
Log.Current.Trace("[SourceReference] Log.Current.Trace");
Log.Current.Debug("[SourceReference] Log.Current.Debug");
Log.Current.Info("[SourceReference] Log.Current.Info");
Log.Current.Warn("[SourceReference] Log.Current.Warn");
Log.Current.Error("[SourceReference] Log.Current.Error");
Log.Current.Fatal("[SourceReference] Log.Current.Fatal");
```

### 源生成器

有时候你希望制作一个库，想使用日志系统却不希望产生日志系统的 NuGet 依赖。

这时，你可以通过如下方式来引用此 NuGet 包：

```xml
<ItemGroup>
   <PackageReference Include="DotNetCampus.Logger" PrivateAssets="all" ExcludeAssets="compile;runtime" />
</ItemGroup>
```

其中：

1. PrivateAssets="all" 用于表明生成的 NuGet 包将不引入 DotNetCampus.Logger 包依赖；
2. ExcludeAssets 中 compile 用于不引入编译时的依赖（此时以下源才会自动解除注释）；
3. ExcludeAssets 中 runtime 用于不将依赖复制到目标目录（因为运行时已经不需要它们了）。

这时，源生成器会在你的项目中生成一套日志系统的内部代码。你可以在库里各处正常使用日志系统的各种类和方法。

特别的，无论你的项目目前是以什么方式使用的此日志系统库，你都可以在源生成器中找到生成的代码。只是对于不需要生成代码的场景下，里面生成的代码是注释掉的；你可以通过注释调查没有生成源代码的原因，以及辅助查阅本文档的更详细细节。

### 日志桥接

如果你的应用程序依赖了一个或多个包含上述源生成器日志系统的库，可以使用桥接功能将它们的日志接到当前应用程序的日志系统中。

以下是一个复杂大型项目的初始化示例代码：

```csharp
using DotNetCampus.Logging.Attributes;
using DotNetCampus.Logging.Writers;

namespace DotNetCampus.Demo.Sample;

internal static class LoggerStartup
{
    public static AppBuilder WithLogger(this AppBuilder builder, string[] args)
    {
        new LoggerBuilder()
            // 在日志初始化之前也可以使用日志系统，这些日志会缓存到内存中，直到日志系统初始化完成后再使用。
            .WithMemoryCache()
            // 设置日志级别为 Debug。
            .WithLevel(LogLevel.Debug)
            // 添加一个控制台日志写入器，这样控制台里就可以看到日志输出了。
            .AddConsoleLogger(b => b
                .WithThreadSafe(LogWritingThreadMode.ProducerConsumer)
                .FilterConsoleTagsFromCommandLineArgs(args))
            // 如果有一些库使用了本日志框架（使用源生成器，不带依赖的那种），那么可以通过这个方法将它们的日志桥接到本日志框架中。
            .AddBridge(LoggerBridgeLinker.Default)
            // 初始化日志系统。
            .Build()
            // 将初始化的日志系统设置到静态全局的 Log 类中，这样就可以在任何地方使用 Log 类来输出日志了。
            .IntoGlobalStaticLog();
        return builder;
    }
}

[ImportLoggerBridge<global::DotNetCampus.Demo1.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::DotNetCampus.Demo2.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
```

桥接类的命名空间由桥接类所在项目的 `$(RootNamespace)` 决定，你也可以通过指定 `$(DCGeneratedLoggerNamespace)` 单独指定桥接类的命名空间而不影响项目原本的命名空间。

另外，在你的应用程序中，日志级别通常不是写死在代码里的，往往需要从命令行参数、环境变量、配置文件等位置读取。那么可通过 `LogLevelParser.Parse` 方法将字符串解析为日志级别，相比于普通枚举的解析，此方法额外支持日志级别的常见别名。支持的别名请参见 [LogLevelParser.cs](https://github.com/dotnet-campus/DotNetCampus.Logger/blob/main/src/DotNetCampus.Logger/LogLevelParser.cs) 的注释。

### 日志过滤规则

当在命令行中传入 `--log-console-tags TagA,TagB` 时，将进行日志过滤，只输出包含 `TagA` 或 `TagB` 的日志。

基本规则如下：

1. 可指定单个或多个标签；多个标签之间使用 `,` 或 `;` 分隔，也可以使用空格分隔（但需要注意命令行参数的引号问题）。
1. 标签可不带前缀，也可带有 `+` 或 `-` 前缀，分别表示包含、不包含该标签。

当指定了多个标签时，更详细的规则如下：

1. 先对所有不带前缀的标签进行匹配：
    - 如果没有指定不带前缀的标签，则所有日志都可以进行后续过滤
    - 如果指定了不带前缀的标签，那么当这些标签中的任何一个能在日志找找到时，这条日志就可以进行后续过滤
2. 然后对所有带有 `+` 前缀的标签进行匹配：
    - 如果没有指定带有 `+` 前缀的标签，则前面过滤出来的所有日志都可以进行后续过滤
    - 如果指定了带有 `+` 前缀的标签，那么当这些标签中的所有标签都能在日志中找到时，这条日志就可以进行后续过滤
3. 最后对所有带有 `-` 前缀的标签进行匹配：
    - 如果没有指定带有 `-` 前缀的标签，则前面过滤出来的所有日志都可以进行后续过滤
    - 如果指定了带有 `-` 前缀的标签，那么当这些标签中的任何一个在日志中找到时，这条日志就在过滤中被排除，排除后剩余的日志才可以进行后续过滤
4. 上述 3 个步骤完成之后，过滤出来的日志就是最终输出的日志

例如，当传入 `--log-console-tags Foo1,Foo2,+Bar1,+Bar2,-Baz1,-Baz2` 时：

1. 只要日志中包含 `Foo1` 或 `Foo2`，就可以进行后续过滤
1. 只有当日志中同时包含 `Bar1` 和 `Bar2` 时，才可以进行后续过滤
1. 只要日志中包含 `Baz1` 或 `Baz2`，就会被排除在外
1. 最终输出的日志是同时满足上述 3 个条件的日志

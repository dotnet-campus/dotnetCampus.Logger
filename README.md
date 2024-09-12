# dotnetCampus.Logger

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetCampus.Logger/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.Logger.svg)](https://www.nuget.org/packages/dotnetCampus.Logger)|

## 入门

### 安装日志库

在你的项目中安装 `dotnetCampus.Logger` 包，你可以通过 NuGet 包管理器或者通过命令行来安装：

```shell
dotnet add package dotnetCampus.Logger
```

安装完成后，你可以在项目中设置属性来决定如何使用日志库：

```xml
<PropertyGroup>
    <!-- 设置以源生成器的方式来使用日志库。 -->
    <!-- 以此方式使用日志库，不会使你的项目产生任何额外的依赖，特别适合用于不希望引入额外依赖的库项目。 -->
    <DCUseGeneratedLogger>true</DCUseGeneratedLogger>
</PropertyGroup>
```

这个属性可选的值有：

- `onlySource`: 只使用源生成器日志系统，不会引用库；
- `preferSource`: 使用源生成器日志系统，并优先使用它；
- `preferReference`: 使用源生成器日志系统，但优先使用引用的库；
- `onlyReference`: 只使用引用的库，不会生成源代码；
- `true`: 只使用源生成器日志系统，不会引用库（其含义与 `onlySource` 等同）；
- `false`: 只使用引用的库，不会生成源代码（其含义与 `onlyReference` 等同）。

在库中，合适的值为 `onlySource` 以不引入依赖；在产品项目中，合适的值为 `onlyReference`，以使用全功能的日志库。

在产品的入口项目中，既可以使用 `onlyReference` 也可以使用 `preferReference`。前者允许你以常规的方式使用日志库，而后者则允许你在初始化日志库之前使用日志库。

### 初始化

在你的初始化代码中添加如下代码，即可完成日志的配置和初始化：

```csharp
new LoggerBuilder()
    .WithLevel(LogLevel.Debug)
    .AddWriter(new ConsoleLogger())
    .Build()
    .IntoGlobalStaticLog();
```

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

对于复杂的大型项目，可能会有更加丰富的需求，可以参考下面的示例代码：

```csharp
using dotnetCampus.Logging.Attributes;
using dotnetCampus.Logging.Writers;

namespace dotnetCampus.Demo.Sample;

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

[ImportLoggerBridge<global::dotnetCampus.Demo1.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::dotnetCampus.Demo2.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
```

当然，在你的应用程序中，日志级别通常不是写死在代码里的，往往需要从命令行参数、环境变量、配置文件等位置读取。那么可通过 `LogLevel.Parse` 方法将字符串解析为日志级别，相比于普通枚举的解析，此方法额外支持日志级别的常见别名。

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

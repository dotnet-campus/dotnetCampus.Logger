using System;

namespace DotNetCampus.Logging.Attributes;

/// <summary>
/// 指示源生成器应该生成对接指定的日志桥接器的代码。
/// </summary>
/// <param name="loggerBridgeInterfaceType">
/// 桥接器的类型。通常名为 global::Xxx.Logging.ILoggerBridge，其中 Xxx 为被桥接的库的根命名空间。
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ImportLoggerBridgeAttribute(Type loggerBridgeInterfaceType) : Attribute
{
    /// <summary>
    /// 获取桥接器的类型。
    /// </summary>
    public Type LoggerBridgeInterfaceType { get; } = loggerBridgeInterfaceType;
}

/// <summary>
/// 指示源生成器应该生成对接指定的日志桥接器的代码。
/// </summary>
/// <typeparam name="T">
/// 桥接器的类型。通常名为 global::Xxx.Logging.BridgeLogger，其中 Xxx 为被桥接的库的根命名空间。
/// </typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ImportLoggerBridgeAttribute<T>() : ImportLoggerBridgeAttribute(typeof(T));

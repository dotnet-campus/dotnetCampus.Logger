#if NET6_0_OR_GREATER
using global::System.Runtime.CompilerServices;

namespace dotnetCampus.Logging;

[InterpolatedStringHandler]
public ref struct LoggerInterpolatedStringHandler
{
    public LoggerInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    }

    private DefaultInterpolatedStringHandler _handler;

    public void AppendLiteral(string s)
    {
        ref var handler = ref _handler;
        handler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value)
    {
        ref var handler = ref _handler;
        handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string format)
    {
        ref var handler = ref _handler;
        handler.AppendFormatted(value, format);
    }

    public string ToStringAndClear()
    {
        ref var handle = ref _handler;
        return handle.ToStringAndClear();
    }

    public override string ToString()
    {
        ref var handler = ref _handler;
        return handler.ToString();
    }

    /// <summary>
    /// 废弃掉此字符串
    /// </summary>
    public void Discard()
    {
        // 这里的 ToStringAndClear 其实只是取其 Clear 的功能
        // 暂时先使用 DefaultInterpolatedStringHandler 提供的能力，后续再考虑是否需要优化
        ref var handler = ref _handler;
        handler.ToStringAndClear();
    }
}

#endif
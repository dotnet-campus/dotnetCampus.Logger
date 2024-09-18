using System.Runtime.CompilerServices;

#if NET6_0_OR_GREATER

namespace dotnetCampus.Logging;

[InterpolatedStringHandler]
public ref struct LoggerInterpolatedStringHandler
{
    public LoggerInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    }

    private readonly DefaultInterpolatedStringHandler _handler;

    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    public void AppendFormatted<T>(T value) => _handler.AppendFormatted(value);

    public void AppendFormatted<T>(T value, string? format) => _handler.AppendFormatted(value, format);

    public string ToStringAndClear() => _handler.ToStringAndClear();

    public override string ToString() => _handler.ToString();

    /// <summary>
    /// 废弃掉此字符串
    /// </summary>
    public void Discard()
    {
        // 这里的 ToStringAndClear 其实只是取其 Clear 的功能
        // 暂时先使用 DefaultInterpolatedStringHandler 提供的能力，后续再考虑是否需要优化
        _handler.ToStringAndClear();
    }
}

#endif

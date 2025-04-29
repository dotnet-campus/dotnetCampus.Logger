#nullable enable
using global::System.Diagnostics.CodeAnalysis;

namespace dotnetCampus.Logging;

/// <summary>
/// 标识一个日志事件。主要标识是 "Id" 属性，而 "Name" 属性提供了此类型事件的简短描述。
/// </summary>
public readonly struct EventId
{
    /// <summary>
    /// 从给定的 <see cref="int"/> 隐式创建一个 EventId。
    /// </summary>
    /// <param name="i">要转换为 EventId 的 <see cref="int"/>。</param>
    public static implicit operator EventId(int i)
    {
        return new EventId(i);
    }

    /// <summary>
    /// 检查两个指定的 <see cref="EventId"/> 实例是否具有相同的值。如果它们具有相同的 Id，则它们是相等的。
    /// </summary>
    /// <param name="left">第一个 <see cref="EventId"/>。</param>
    /// <param name="right">第二个 <see cref="EventId"/>。</param>
    /// <returns>如果对象相等，则为 <see langword="true" />。</returns>
    public static bool operator ==(EventId left, EventId right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 检查两个指定的 <see cref="EventId"/> 实例是否具有不同的值。
    /// </summary>
    /// <param name="left">第一个 <see cref="EventId"/>。</param>
    /// <param name="right">第二个 <see cref="EventId"/>。</param>
    /// <returns>如果对象不相等，则为 <see langword="true" />。</returns>
    public static bool operator !=(EventId left, EventId right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// 初始化 <see cref="EventId"/> 结构的新实例。
    /// </summary>
    /// <param name="id">此事件的数字标识符。</param>
    /// <param name="name">此事件的名称。</param>
    public EventId(int id, string? name = null)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 获取此事件的数字标识符。
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 获取此事件的名称。
    /// </summary>
    public string? Name { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name ?? Id.ToString();
    }

    /// <summary>
    /// 指示当前对象是否等于另一个相同类型的对象。如果两个事件具有相同的 id，则它们是相等的。
    /// </summary>
    /// <param name="other">要与此对象进行比较的对象。</param>
    /// <returns>如果两个对象相等，则为 <see langword="true" />；否则为 <see langword="false" />。</returns>
    public bool Equals(EventId other)
    {
        return Id == other.Id;
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        return obj is EventId eventId && Equals(eventId);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id;
    }
}

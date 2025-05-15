using System;

namespace DotNetCampus.Logging.Configurations;

internal class InheritedConfiguration<T>(T options)
    where T : notnull
{
    private InheritedConfiguration<T>? _parent;

    internal void AddChild<TChild>(TChild child)
        where TChild : InheritedConfiguration<T>
    {
        child._parent = this;
    }

    internal TValue GetValue<TValue>(Func<T, TValue> getter, TValue defaultValue = default!)
    {
        var value = getter(options);
        if (value is not null)
        {
            return value;
        }

        if (_parent is not null)
        {
            return _parent.GetValue(getter, defaultValue);
        }

        return defaultValue;
    }

    internal void SetValue<TValue>(Func<T, TValue> setter)
    {
        setter(options);
    }
}

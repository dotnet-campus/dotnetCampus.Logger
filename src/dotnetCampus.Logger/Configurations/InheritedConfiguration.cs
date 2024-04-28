namespace dotnetCampus.Logging.Configurations;

internal class InheritedConfiguration<T> where T : notnull
{
    private InheritedConfiguration<T>? _parent;

    public void AddChild<TChild>(TChild child)
        where TChild : InheritedConfiguration<T>
    {
        child._parent = this;
    }
}

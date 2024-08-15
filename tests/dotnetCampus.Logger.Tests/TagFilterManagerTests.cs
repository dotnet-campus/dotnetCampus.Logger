using dotnetCampus.Logging.Writers.Helpers;

namespace dotnetCampus.Logger.Tests;

[TestClass]
public class TagFilterManagerTests
{
    [TestMethod("单个任一标签，只要有一个标签匹配即允许。")]
    public void 单个任一标签()
    {
        var filter = CreateFilter("Foo");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("多个任一标签，只要有一个标签匹配即允许。")]
    public void 多个任一标签()
    {
        var filter = CreateFilter("Foo,Bar");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("单个包含标签，只要有一个标签匹配即允许。")]
    public void 单个包含标签()
    {
        var filter = CreateFilter("+Foo");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("多个包含标签，必须所有标签都匹配才允许。")]
    public void 多个包含标签()
    {
        var filter = CreateFilter("+Foo,+Bar");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("单个排除标签，任一标签匹配即不允许。")]
    public void 单个排除标签()
    {
        var filter = CreateFilter("-Foo");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("多个排除标签，任一标签匹配即不允许。")]
    public void 多个排除标签()
    {
        var filter = CreateFilter("-Foo,-Bar");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx] Message"));
    }

    private static TagFilterManager CreateFilter(string commandLineFilterValue)
    {
        return TagFilterManager.FromCommandLineArgs([TagFilterManager.LogTagParameterName, commandLineFilterValue])!;
    }
}

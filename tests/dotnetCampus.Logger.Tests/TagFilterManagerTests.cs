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

    [TestMethod("单个任一和包含标签，只要有一个标签匹配即允许。")]
    public void 单个任一和包含标签()
    {
        var filter = CreateFilter("Foo,+Bar");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("单个任一和排除标签，只要有一个标签匹配即不允许。")]
    public void 单个任一和排除标签()
    {
        var filter = CreateFilter("Foo,-Bar");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("单个包含和排除标签，只要有一个标签匹配即不允许。")]
    public void 单个包含和排除标签()
    {
        var filter = CreateFilter("+Foo,-Bar");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("单个任一和包含和排除标签，只要有一个排除标签匹配即不允许，否则只要有一个标签匹配即允许。")]
    public void 单个任一和包含和排除标签()
    {
        var filter = CreateFilter("Foo,+Bar,-Baz");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Baz] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[xxxx][Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo][Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Bar][Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx][Foo][Bar][Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[xxxx] Message"));
    }

    [TestMethod("多个任一和包含标签，在任一标签匹配的基础上，必须所有包含标签匹配才允许。")]
    public void 多个任一和包含标签()
    {
        var filter = CreateFilter("Foo,Bar,+Baz,+Qux");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Baz] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Baz][Qux] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar][Baz][Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz][Qux] Message"));
    }

    [TestMethod("多个任一和排除标签，在任一标签匹配的基础上，只要有一个排除标签匹配即不允许。")]
    public void 多个任一和排除标签()
    {
        var filter = CreateFilter("Foo,Bar,-Baz,-Qux");
        Assert.IsTrue(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Qux] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Baz][Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar][Baz][Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz][Qux] Message"));
    }

    [TestMethod("多个包含和排除标签，必须所有包含标签匹配，且没有排除标签匹配才允许。")]
    public void 多个包含和排除标签()
    {
        var filter = CreateFilter("+Foo,+Bar,-Baz,-Qux");
        Assert.IsFalse(filter.IsTagEnabled("[Foo] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Qux] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo][Bar] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Baz][Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo][Bar][Baz][Qux] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz][Qux] Message"));
    }

    public void 多个任一和包含和排除标签()
    {
        var filter = CreateFilter("Foo1,Foo2,+Bar1,+Bar2,-Baz1,-Baz2");
        Assert.IsFalse(filter.IsTagEnabled("[Foo1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Bar2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Foo2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Baz2] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo1][Bar1][Bar2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar2][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar2][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Bar2][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Bar2][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Baz1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar2][Baz1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo1][Bar1][Bar2][Baz1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Baz2] Message"));
        Assert.IsTrue(filter.IsTagEnabled("[Foo2][Bar1][Bar2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar2][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar2][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Bar2][Baz1] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Bar2][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Baz1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar2][Baz1][Baz2] Message"));
        Assert.IsFalse(filter.IsTagEnabled("[Foo2][Bar1][Bar2][Baz1][Baz2] Message"));
    }

    private static TagFilterManager CreateFilter(string commandLineFilterValue)
    {
        return TagFilterManager.FromCommandLineArgs([TagFilterManager.LogTagParameterName, commandLineFilterValue])!;
    }
}

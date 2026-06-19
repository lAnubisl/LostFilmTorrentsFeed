namespace LostFilmMonitoring.BLL.Tests;

[ExcludeFromCodeCoverage]
internal static class TestAssert
{
    public static void HasSingleError(Dictionary<string, string> errors, string key, string value)
    {
        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors.Single().Key, Is.EqualTo(key));
        Assert.That(errors.Single().Value, Is.EqualTo(value));
    }
}

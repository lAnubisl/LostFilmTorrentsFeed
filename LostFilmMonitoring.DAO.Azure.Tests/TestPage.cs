namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestPage<T> : Page<T>
{
    private readonly T[] values;

    public TestPage(T[] values)
    {
        this.values = values;
    }

    public override IReadOnlyList<T> Values => values;

    public override string? ContinuationToken => null;

    public override Response GetRawResponse() => new TestResponseOk();
}

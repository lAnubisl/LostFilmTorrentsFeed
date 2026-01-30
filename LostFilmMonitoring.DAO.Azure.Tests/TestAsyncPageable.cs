namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestAsyncPageable<T> : AsyncPageable<T> where T: notnull
{
    private readonly T[] values;

    public TestAsyncPageable(T[] values)
    {
        this.values = values;
    }

    public override IAsyncEnumerable<Page<T>> AsPages(string? continuationToken = null, int? pageSizeHint = null)
    {
        return new TestAsyncEnumerable<Page<T>>([new TestPage<T>(values)]);
    }
}

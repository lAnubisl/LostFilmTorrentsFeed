namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly T[] values;
    
    public TestAsyncEnumerable(T[] values)
    {
        this.values = values;
    }
    
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.values);
    }
}

namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerable<T> values;
    private readonly IEnumerator<T> enumerator;
    
    public TestAsyncEnumerator(T[] values)
    {
        this.values = values;
        this.enumerator = this.values.GetEnumerator();
    }
    
    public T Current => enumerator.Current;

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(enumerator.MoveNext());
    }
}

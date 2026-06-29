namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly T[] values;
    private readonly IEnumerator<T> enumerator;
    
    public TestAsyncEnumerator(T[] values)
    {
        this.values = values;
        this.enumerator = ((IEnumerable<T>)this.values).GetEnumerator();
    }
    
    public T Current => enumerator.Current;

    public ValueTask DisposeAsync()
    {
        this.enumerator?.Dispose();
        GC.SuppressFinalize(this);
        return new ValueTask();
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(enumerator.MoveNext());
    }
}

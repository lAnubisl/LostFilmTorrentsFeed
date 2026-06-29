namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestResponseOk : Response
{
    public override int Status => 200;

    public override string ReasonPhrase => string.Empty;

    public override Stream? ContentStream { get; set; }
    public override string ClientRequestId { get; set; } = string.Empty;

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    protected override bool ContainsHeader(string name)
    {
        return false;
    }

    protected override IEnumerable<HttpHeader> EnumerateHeaders()
    {
        return Enumerable.Empty<HttpHeader>();
    }

    protected override bool TryGetHeader(string name, [NotNullWhen(true)] out string? value)
    {
        value = null;
        return false;
    }

    protected override bool TryGetHeaderValues(string name, [NotNullWhen(true)] out IEnumerable<string>? values)
    {
        throw new NotImplementedException();
    }
}

namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class TestResponse<T> : Response<T>
{
    private readonly T expectedObject;
    
    public TestResponse(T expectedObject)
    {
        this.expectedObject = expectedObject;
    }

    public override T Value => this.expectedObject;

    public override Response GetRawResponse() => new TestResponseOk();
}

namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageModelPersisterTests
{
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }
    
    private static string GetFile(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}

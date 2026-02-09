namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageModelPersisterTests
{
    [SetUp]
    public void Setup()
    {
        var logger = new Mock<Common.ILogger>();
        logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
    }
    
    private static string GetFile(string fileName)
    {
        var assembly = typeof(AzureBlobStorageModelPersisterTests).Assembly;
        var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
